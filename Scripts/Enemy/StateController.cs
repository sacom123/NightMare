using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NM;


public class StateController : MonoBehaviour
{
    public GeneralStats generalStats;
    public ClassStats statData;
    
   
    [Tooltip("적 오브젝트")]
    public GameObject[] EnemyObeject;
    [Tooltip("적 무기")]
    public GameObject[] EnemyWeapon;
    [Tooltip("보스 State")]
    public State BossState;

    [Tooltip("현재 상태")]
    public State currentState;
    [Tooltip("현재 상태 반복")]
    public State remainState;
    [Tooltip("적의 목표 타겟")]
    public Transform aimTarget;
    [Tooltip("정찰할 포인트들")]
    public List<Transform> patrolWayPoints;
    [Tooltip("최대 몇번 공격할 것?")]
    public int AttackCount;

    [Tooltip("적의 시야를 Scene에 그려줌")]
    [Range(0, 50)]
    public float viewRadius;
    [Tooltip("적의 시야를 Scene에 그려줌")]
    [Range(0, 360)]
    public float viewAngle;
    [Tooltip("적들이 player를 인식할 반경을 Scene에 그려줌")]
    [Range(0, 25)]
    public float perceptionRadius;

    //Way Point 관련
    [HideInInspector] public NavMeshAgent nav;      // 네이게이션을 쓸 예정
    [HideInInspector] public int wayPointIndex;         // wayPoint index 
    [HideInInspector] public int WayPointHash = -1;
    [HideInInspector] public float waitTime = 0f;       // 기다리고 있는 초
    [HideInInspector] public float MaxWaitTime = 5f;    // 몇초를 기다릴 것인가

    //enemy type 관련
    [HideInInspector] public int random;
    [HideInInspector] public int EnemyTypeint;
    
    public Avatar bossAvatar;
    public RuntimeAnimatorController bossAnimator;


    public GameObject EnemyBone;

    //Attack 관련
    [HideInInspector] public int magAttack;              // 최대로 공격할 횟수를 담고 있음, xlsx 파일에서 관리 하는데 여기서도 Random값을 줘서 다양성을 부여
    [HideInInspector] public int currentAttackTime = 0;  // 공격을 몇초간 하고 있는가
    [HideInInspector] public bool isAttacking = false;   // 공격이 실행 중인가 아닌가
    [HideInInspector] public int MaximumAttackCount;     // 최대 몇번의 공격을 시행할 것인가
    [HideInInspector] public int currentAttackCount = 0; // 현재 몇번 공격을 시행했는가
    [HideInInspector] public bool attackDelay;           // 공격 딜레이가 걸렸는가
    [HideInInspector] public bool AttackStateDelay = false;
    [HideInInspector] public GameObject Weapon;           // 현재 지닌 무기에 대한 GameObject
    [HideInInspector] public float attackStateDelay = 0f; // Attack 사이 간격을 줄 float 형 value


    [HideInInspector] public Transform playerTransform; //player의 위치를 받아 오기 위함

    //Target이 가까운 정도를 확인
    [HideInInspector] public float nearRadius;
    // 추적이 불가능할때 30초가 지나면 Patrol상태로 전환
    [HideInInspector] public float blindEngageTime = 30f;
    // 타것이 시야에 있는가
    [HideInInspector] public bool targetInSight;
    // 에임이 고정되어 있는가
    [HideInInspector] public bool focusSight;

    //아마 안쓸듯? 확인 후 지울 예정
    [HideInInspector] public bool hadClearShot; //before
    [HideInInspector] public bool haveClarshot; // now
    //Enemy Animation과 Variables 스크립트를 사용하기 위해 설정
    [HideInInspector] public EnemyAnimation enemyAnimation;
    [HideInInspector] public float originAnimSpeed;
    [HideInInspector] public EnemyVariables variables;
    [HideInInspector] public EnemyHP enemyhelth;
    //Enemy 개개인 객체 마다의 Target을 지정해주기 위함
    [HideInInspector] public Vector3 personalTarget = Vector3.zero;

    [HideInInspector] public int effecid;

    [HideInInspector] public EnemySpawnManager ESM;

    [HideInInspector]public bool aiActive; // ai를 끄고 킬 bool 값
    private bool strafing; // Strafing을 할 것인가에 대한 bool 값
    private bool aiming;   // aiming중인가 아닌가

    
    /// <summary>
    /// 시작 하자마자 적의 이름 or 태그를 검색 해서 classID에 삽입
    /// 이후에 랜덤 값을 받아와서 classID를 부여해준다.
    /// 0,1은 Sword, 2,3은 Magic, 4,5는 근접 Elite로 EliteS, 6,7은 EliteM, 8은 Boss 총 9개, 0~8까지의 랜덤값이며 보스의 경우 보스 스테이지 갔을때
    /// 랜덤 값이 아닌 8을 반환 하게 설정
    /// 추후에 Random 값을 넘겨 받아 4,5,6,7 이면 scale값을 1.5~2배 정도 늘린다
    /// EliteS 면 2, EliteM이면 1.5배로 예상중
    /// 정확히는 Random 값 보다는 ClassId를 검색하는게 맞을거 같긴함 이건 상의 후 결정
    /// </summary>                 0       1        2      3        4         5        6        7      8 인데 8일 경우 별도의 스크립트로 전환
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

        //게임의 다양성을 주기 위함, 공격 가능 횟수를 /2~attackCount까지 부여
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
    /// 메쉬의 경우 엘리트 몹은 기존 몹 메쉬에서 2배를 할거기 때문에 -4를 하면 맞음
    /// 메쉬가 근접 1,2 원거리 1,2 이렇게 있기 때문에 0~3까지 할당 되어있음 원래라면 0~7까지 인데 위의 이유를 들어
    /// -4를 하면 됨
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

            //EnemyClass와 class Id 를 비교해서 나오는 int 값이 Elite 몬스터가 있는 배열에 해당하면
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

    //엑셀 파일에서 에셋으로 변환 시킨 값에서 해당 classId에 해당하는 값을 불러와줌
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

    
    //state 를 Decsion에 따라서 교체
    public void TransitionToState(State nextState, Decision decision)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    //Strafing 관리
    public bool Strafing
    {
        get => strafing;
        set
        {
            enemyAnimation.anim.SetBool("Strafe",value);
            strafing = value;
        }
    }

    //조준 중인 것을 관리
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

    //에임이 꼬였거나 풀어야 할때 사용하는 함수 
    public IEnumerator UnstuckAim(float delay)
    {
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = false;
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = true;
    }
    //AI 실행
    
    //ai가 켜져 있으면 현재 상태의 액션을 실행하고 트랜지션을 확인해주기 위한 Update 함수
    private void Update()
    {
        if (!aiActive)
        {
            return;
        }
        currentState.DoActions(this);
        currentState.CheckTransitions(this);
    }

    //Enemy의 머리 위에 Scene에서 현재 State의 변화를 확인하기 위한 Gizmo를 그려줌
    private void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 3f);
        }
    }

}
