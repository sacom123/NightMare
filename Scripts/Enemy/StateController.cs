using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NM;


public class StateController : MonoBehaviour
{
    public GeneralStats generalStats;
    public ClassStats statData;
    
   
    [Tooltip("�� ������Ʈ")]
    public GameObject[] EnemyObeject;
    [Tooltip("�� ����")]
    public GameObject[] EnemyWeapon;
    [Tooltip("���� State")]
    public State BossState;

    [Tooltip("���� ����")]
    public State currentState;
    [Tooltip("���� ���� �ݺ�")]
    public State remainState;
    [Tooltip("���� ��ǥ Ÿ��")]
    public Transform aimTarget;
    [Tooltip("������ ����Ʈ��")]
    public List<Transform> patrolWayPoints;
    [Tooltip("�ִ� ��� ������ ��?")]
    public int AttackCount;

    [Tooltip("���� �þ߸� Scene�� �׷���")]
    [Range(0, 50)]
    public float viewRadius;
    [Tooltip("���� �þ߸� Scene�� �׷���")]
    [Range(0, 360)]
    public float viewAngle;
    [Tooltip("������ player�� �ν��� �ݰ��� Scene�� �׷���")]
    [Range(0, 25)]
    public float perceptionRadius;

    //Way Point ����
    [HideInInspector] public NavMeshAgent nav;      // ���̰��̼��� �� ����
    [HideInInspector] public int wayPointIndex;         // wayPoint index 
    [HideInInspector] public int WayPointHash = -1;
    [HideInInspector] public float waitTime = 0f;       // ��ٸ��� �ִ� ��
    [HideInInspector] public float MaxWaitTime = 5f;    // ���ʸ� ��ٸ� ���ΰ�

    //enemy type ����
    [HideInInspector] public int random;
    [HideInInspector] public int EnemyTypeint;
    
    public Avatar bossAvatar;
    public RuntimeAnimatorController bossAnimator;


    public GameObject EnemyBone;

    //Attack ����
    [HideInInspector] public int magAttack;              // �ִ�� ������ Ƚ���� ��� ����, xlsx ���Ͽ��� ���� �ϴµ� ���⼭�� Random���� �༭ �پ缺�� �ο�
    [HideInInspector] public int currentAttackTime = 0;  // ������ ���ʰ� �ϰ� �ִ°�
    [HideInInspector] public bool isAttacking = false;   // ������ ���� ���ΰ� �ƴѰ�
    [HideInInspector] public int MaximumAttackCount;     // �ִ� ����� ������ ������ ���ΰ�
    [HideInInspector] public int currentAttackCount = 0; // ���� ��� ������ �����ߴ°�
    [HideInInspector] public bool attackDelay;           // ���� �����̰� �ɷȴ°�
    [HideInInspector] public bool AttackStateDelay = false;
    [HideInInspector] public GameObject Weapon;           // ���� ���� ���⿡ ���� GameObject
    [HideInInspector] public float attackStateDelay = 0f; // Attack ���� ������ �� float �� value


    [HideInInspector] public Transform playerTransform; //player�� ��ġ�� �޾� ���� ����

    //Target�� ����� ������ Ȯ��
    [HideInInspector] public float nearRadius;
    // ������ �Ұ����Ҷ� 30�ʰ� ������ Patrol���·� ��ȯ
    [HideInInspector] public float blindEngageTime = 30f;
    // Ÿ���� �þ߿� �ִ°�
    [HideInInspector] public bool targetInSight;
    // ������ �����Ǿ� �ִ°�
    [HideInInspector] public bool focusSight;

    //�Ƹ� �Ⱦ���? Ȯ�� �� ���� ����
    [HideInInspector] public bool hadClearShot; //before
    [HideInInspector] public bool haveClarshot; // now
    //Enemy Animation�� Variables ��ũ��Ʈ�� ����ϱ� ���� ����
    [HideInInspector] public EnemyAnimation enemyAnimation;
    [HideInInspector] public float originAnimSpeed;
    [HideInInspector] public EnemyVariables variables;
    [HideInInspector] public EnemyHP enemyhelth;
    //Enemy ������ ��ü ������ Target�� �������ֱ� ����
    [HideInInspector] public Vector3 personalTarget = Vector3.zero;

    [HideInInspector] public int effecid;

    [HideInInspector] public EnemySpawnManager ESM;

    [HideInInspector]public bool aiActive; // ai�� ���� ų bool ��
    private bool strafing; // Strafing�� �� ���ΰ��� ���� bool ��
    private bool aiming;   // aiming���ΰ� �ƴѰ�

    
    /// <summary>
    /// ���� ���ڸ��� ���� �̸� or �±׸� �˻� �ؼ� classID�� ����
    /// ���Ŀ� ���� ���� �޾ƿͼ� classID�� �ο����ش�.
    /// 0,1�� Sword, 2,3�� Magic, 4,5�� ���� Elite�� EliteS, 6,7�� EliteM, 8�� Boss �� 9��, 0~8������ �������̸� ������ ��� ���� �������� ������
    /// ���� ���� �ƴ� 8�� ��ȯ �ϰ� ����
    /// ���Ŀ� Random ���� �Ѱ� �޾� 4,5,6,7 �̸� scale���� 1.5~2�� ���� �ø���
    /// EliteS �� 2, EliteM�̸� 1.5��� ������
    /// ��Ȯ���� Random �� ���ٴ� ClassId�� �˻��ϴ°� ������ ������ �̰� ���� �� ����
    /// </summary>                 0       1        2      3        4         5        6        7      8 �ε� 8�� ��� ������ ��ũ��Ʈ�� ��ȯ
    public string classID; // Warrior1, Warrior2, Magic1, Magic2, EliteS1, EliteS2, EliteM1, EliteM2, Boss
    public string WeaponType;

    public void Awake()
    {
        ESM = GameObject.Find("EnemySpawnManager").GetComponent<EnemySpawnManager>();
        
        enemyAnimation = gameObject.AddComponent<EnemyAnimation>();
        
        aimTarget = GameObject.FindGameObjectWithTag("Player").transform;

        enemyhelth = GetComponent<EnemyHP>();
        
        nav = GetComponent<NavMeshAgent>();

        originAnimSpeed = 1f;

        attackDelay = false;

        
    }
   void Start()
    {
        //classID = (string)System.Enum.GetName(typeof(EnemyClass), ESM.EnemyTypecount[EnemyTypeint]);

        enemyhelth.enemyMaxHP = classStats.HP;

        enemyhelth.HpSet();

        WeaponType = classStats.WeponType;

        effecid = ((int)System.Enum.Parse(typeof(WeaponType), classStats.WeponType));

        aiActive = true;

        currentState.OnEnableActions(this);

        WeaponType = classStats.WeponType;

        //������ �پ缺�� �ֱ� ����, ���� ���� Ƚ���� /2~attackCount���� �ο�
        AttackCount = Random.Range(classStats.AttackCount / 2, classStats.AttackCount);

        magAttack = AttackCount;

        variables.ShotsInRounds = magAttack;

        perceptionRadius = classStats.Range;

        nearRadius = perceptionRadius * 0.5f;

        viewRadius = classStats.Range;

        nav.stoppingDistance = classStats.Range;
        
    }

    private void BossSet(int num)
    {
        if ((int)System.Enum.Parse(typeof(EnemyClass), classID) == 8)
        {
            GetComponent<Animator>().avatar = bossAvatar;
            EnemyObeject[num].SetActive(true);
            transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            currentState = BossState;
            EnemyBone.SetActive(false);
            GetComponent<Animator>().runtimeAnimatorController = bossAnimator;
        }
    }


    //Enemy mesh Change, With ClassID
    /// <summary>
    /// �޽��� ��� ����Ʈ ���� ���� �� �޽����� 2�踦 �Ұű� ������ -4�� �ϸ� ����
    /// �޽��� ���� 1,2 ���Ÿ� 1,2 �̷��� �ֱ� ������ 0~3���� �Ҵ� �Ǿ����� ������� 0~7���� �ε� ���� ������ ���
    /// -4�� �ϸ� ��
    /// </summary>
    public void ChangeMesh(int random)
    {
        if (random < 8)
        {
            if(random != 0)
            {
                EnemyObeject[0].SetActive(false);
            }
            EnemyObeject[random].SetActive(true);
            GetComponent<EnemyHP>().Render = EnemyObeject[random].GetComponentInChildren<SkinnedMeshRenderer>();
            EnemyWeapon[(int)System.Enum.Parse(typeof(WeaponType), classStats.WeponType)].SetActive(true);

            //EnemyClass�� class Id �� ���ؼ� ������ int ���� Elite ���Ͱ� �ִ� �迭�� �ش��ϸ�
            if ((int)System.Enum.Parse(typeof(EnemyClass), classID) == 4 || (int)System.Enum.Parse(typeof(EnemyClass), classID) == 5 ||
           (int)System.Enum.Parse(typeof(EnemyClass), classID) == 6 || (int)System.Enum.Parse(typeof(EnemyClass), classID) == 7)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            Weapon = EnemyWeapon[(int)System.Enum.Parse(typeof(WeaponType), classStats.WeponType)];
        }

        else if (random == 8)
        {
            BossSet(random);
        }
    }


    private ClassStats.Param cachedClassID = null;

    //���� ���Ͽ��� �������� ��ȯ ��Ų ������ �ش� classId�� �ش��ϴ� ���� �ҷ�����
    public ClassStats.Param classStats
    {
        get
        {
            if (cachedClassID != null)
            {
                return cachedClassID;
            }
            foreach (ClassStats.Sheet sheet in statData.sheets)
            {
                foreach (ClassStats.Param parm in sheet.list)
                {
                    if (parm.ID.Equals(this.classID))
                    {
                        cachedClassID = parm;
                        return cachedClassID;
                    }
                }
            }
            return null;
        }
    }

    
    //state �� Decsion�� ���� ��ü
    public void TransitionToState(State nextState, Decision decision)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    //Strafing ����
    public bool Strafing
    {
        get => strafing;
        set
        {
            enemyAnimation.anim.SetBool("Strafe",value);
            strafing = value;
        }
    }

    //���� ���� ���� ����
    public bool Aiming
    {
        get => aiming;
        set
        {
            if(aiming != value)
            {
                enemyAnimation.anim.SetBool("Aim", value);
                aiming = value;
            }
        }
    }

    //������ �����ų� Ǯ��� �Ҷ� ����ϴ� �Լ� 
    public IEnumerator UnstuckAim(float delay)
    {
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = false;
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = true;
    }
    //AI ����
    
    //ai�� ���� ������ ���� ������ �׼��� �����ϰ� Ʈ�������� Ȯ�����ֱ� ���� Update �Լ�
    private void Update()
    {
        if (!aiActive)
        {
            return;
        }
        currentState.DoActions(this);
        currentState.CheckTransitions(this);
    }

    //Enemy�� �Ӹ� ���� Scene���� ���� State�� ��ȭ�� Ȯ���ϱ� ���� Gizmo�� �׷���
    private void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 3f);
        }
    }

}
