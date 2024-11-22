using NM;
using UnityEngine;

[CreateAssetMenu(menuName = ("EnemyAi/Actions/Attack"))]

public class AttackAction : Action
{
    private readonly float startShootDelay = 0.3f;

    private readonly float aimAngleGap = 30f;

    private float attackDelayCount = 0f;

    private ParticleSystem particleSystem;

    //[HideInInspector] public GameObject Magic;


    [HideInInspector] public int effectId;
    
    //fireball
    [HideInInspector] public float magicSpeed = 5f;
    
    public PlayerFollowAction PFA;

    //�⺻������ �����ؾ��� �Լ���
    public override void OnReadyAction(StateController controller)
    {

        controller.enemyAnimation.anim.SetFloat("AttackSpeed", 1f);

        //�ɾ��־����� �ʱ�ȭ
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);

        //Enemy�� navmesh agent�� speed�� 0���� ����
        controller.nav.speed = 0f;

        controller.enemyAnimation.ActivatePendingAim(); //���� ���, �þ� + �����ȿ� ������ ���� ����
        
        effectId = controller.effecid;

    }
    //�������� ���ϴ� �Լ�
    private void EnemyDealing(StateController controller, Transform target, bool organic)
    {
        if (target && organic)
        {
            Player player = target.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage(controller.classStats.Damaege);
            }
        }
    }

    //���� Ÿ��1�� ���� �ִϸ��̼ǰ� ��ƼŬ ����
    private void SwordanimationAct(StateController controller, int effectId, bool organic, Transform target, Vector3 direction)
    {
        controller.enemyAnimation.anim.SetTrigger(AnimatorKey.Attack);
        controller.enemyAnimation.anim.speed = 0.6f;

        //Effect Object pulling
        GameObject Effect = EffectManager.Instance.EffectOneShot(effectId, controller.enemyAnimation.effectTransform.position);

        Effect.transform.SetParent(controller.transform);
        
        Effect.GetComponent<EnemySwordAttack>().Type = controller.EnemyTypeint;
        Effect.GetComponent<EnemySwordAttack>().controller = controller;
        Vector3 origin2 = controller.enemyAnimation.effectTransform.position;
        Effect.transform.position = origin2;

        Effect.transform.SetPositionAndRotation(origin2, controller.enemyAnimation.effectTransform.rotation);

        Effect.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        particleSystem = Effect.GetComponent<ParticleSystem>();
        var mainModule = particleSystem.main;
        mainModule.simulationSpeed = 0.2f;

        DestroyDelayed destroyDelayed = Effect.AddComponent<DestroyDelayed>();
        destroyDelayed.DelayTime = 2f; //���� �ӵ��� ���缭 ���Ŀ� ����
        controller.attackDelay = true;
        EnemyDealing(controller, target, organic);
    }

    //���̾ �߻�
    private void MagicAnimationAct(StateController controller, int effectId)
    {
        controller.enemyAnimation.anim.SetTrigger(AnimatorKey.Attack);

        GameObject Magic = EffectManager.Instance.EffectOneShot(effectId, controller.enemyAnimation.MagicTransform.position);

        Magic.name = "FireBall";

        Destroy(Magic, 3.0f);

        Magic.GetComponent<EnemyBullet>().Type = controller.EnemyTypeint;

        Magic.GetComponent<EnemyBullet>().controller = controller;

        Vector3 dir = (controller.aimTarget.position - controller.enemyAnimation.MagicTransform.position).normalized;

        dir.y = dir.y + 0.05f;

        Rigidbody rigidbody = Magic.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.velocity = dir * magicSpeed;
            rigidbody.AddForce(dir * magicSpeed * 2f);
        }
        
        controller.attackDelay = true;
    }

    
    // 2��° Ÿ���� �߻� �߰�
    private void Magic2AnimationAct(StateController controller, int effecId)
    {
        controller.enemyAnimation.anim.SetTrigger(AnimatorKey.Attack);
        GameObject Magic = EffectManager.Instance.EffectOneShot(effectId, controller.enemyAnimation.MagicTransform.position);
        Magic.name = "Magic";
        Magic.GetComponent<Magic2PlayTime>().Type = controller.EnemyTypeint;
        Magic.GetComponent<Magic2PlayTime>().controller = controller;
        Magic.transform.rotation = controller.enemyAnimation.MagicTransform.rotation;
        
        controller.attackDelay = true;
        Destroy(Magic, 3.0f);
    }

    // �޾ƿ� Vector3 ����� bool, transform���� ������� effectID�� �˻��ؼ� �ش� Ÿ�Կ� �ش��ϴ� case�� ���� ������ ����
    private void DoAttack(StateController controller, Vector3 direction, Vector3 hitPoint,
        Vector3 hitNormal = default, bool organic = false, Transform target = null)
    {
        if (controller.attackDelay == false)
        {
            switch (effectId)
            {
                case 0:
                    SwordanimationAct(controller, effectId, organic, target, direction);
                    controller.currentAttackCount++;
                    break;
                case 1:
                    SwordanimationAct(controller, effectId, organic, target, direction);
                    controller.currentAttackCount++;
                    break;
                case 2:
                    MagicAnimationAct(controller, effectId);
                    controller.currentAttackCount++;
                    break;
                case 3:
                    Magic2AnimationAct(controller, effectId);
                    controller.currentAttackCount++;
                    break;

            }
            controller.attackDelay = true;
        }
    }

    // Ray Cast�� ����ؼ� ������ ��� ��ġ ��Ȯ�� �ľ�
    private void CastShot(StateController controller)
    {
        Vector3 imprecision = Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.right;
        imprecision += Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.up;

        Vector3 AttackDirection = controller.aimTarget.position - controller.enemyAnimation.transform.position;
        AttackDirection = AttackDirection.normalized + imprecision;
        Ray ray = new Ray(controller.enemyAnimation.transform.position, AttackDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, controller.viewRadius, controller.generalStats.shotMask.value))
        {
            bool isOrganic = ((1 << hit.transform.root.gameObject.layer) & controller.generalStats.targetMask) != 0;
            DoAttack(controller, ray.direction, hit.point, hit.normal, isOrganic, hit.transform);
        }
        else
        {
            DoAttack(controller, ray.direction, ray.origin + (ray.direction * 500f));
        }
    }


    // ������ ���������� ���� ���� Ȯ�� EX) �Ÿ�, ����, classStats���� ���� Range �� �ȿ� ��� �Դ°� 
    private bool CanAttack(StateController controller)
    {
        //���� �� �� �ִ� �����ΰ�, �ʹ� ������ �ʰų� Ÿ�̸� ����
        float distance = (controller.aimTarget.position - controller.enemyAnimation.transform.position).sqrMagnitude;

        if (controller.currentAttackCount <= controller.variables.ShotsInRounds
            && controller.enemyAnimation.currentAimAngleGap < aimAngleGap || distance <= controller.classStats.Range)
        {
            if (controller.variables.startshotTimer >= startShootDelay)
            {
                controller.variables.startshotTimer = 0f;
                return true;
            }
            else
            {
                controller.variables.startshotTimer += Time.deltaTime;
            }
        }
        return false;
    }

    // ���� ������ Ƚ���� �� ������ �� �ִ� Ƚ�� ��, ������ ����������, ���� ���� ������ �ƴ��� �� �˻�
    private void Attack(StateController controller)
    {
        if (controller.currentAttackCount >= controller.variables.ShotsInRounds)
        {
            controller.AttackStateDelay = true;
        }
        //castshot �Լ� ȣ�� �� shotTimer�� 1 ����
        if (Time.timeScale > 0 && controller.currentAttackCount <= controller.variables.ShotsInRounds && controller.isAttacking == false && controller.variables.shotTimer < 1)
        {
            CastShot(controller);
            controller.isAttacking = true;
            controller.variables.shotTimer += 1;
        }
        //������ ���� ���� ��� CurrentShots ���� ��Ű�� shotTimer�� 0���� �ʱ�ȭ
        else if (controller.variables.shotTimer >= 1)
        {
            controller.variables.shotTimer = 0;
            return;
        }
    }

    //�����߿� �����̸� �ɾ��� �ɾ� ���� ������ �ʹ� ������ �����ؼ� �ִϸ��̼��� �������� �ǰ� ������ ����, ���� �ִϸ��̼��� ������ �ʾҴµ��� �ǰ� ���δ�.
    public void attackDelay(StateController controller)
    {
        if ((effectId == 2 || effectId == 3))
        {
            if (attackDelayCount >= controller.classStats.AttackDelay)
            {
                attackDelayCount = 0;
                controller.attackDelay = false;
                controller.isAttacking = false;
            }
        }
        else if (effectId == 0 || effectId == 1)
        {
            if (attackDelayCount >= controller.classStats.AttackDelay)
            {
                attackDelayCount = 0;
                controller.attackDelay = false;
                controller.isAttacking = false;
            }
        }
        attackDelayCount += Time.deltaTime;
    }

    //���� ���� �ൿ�� �ϴ� �Լ� update �� ���� ������ �Ѵ�
    public override void Act(StateController controller)
    {
        controller.focusSight = true;
        effectId = controller.effecid;

        if (CanAttack(controller) && controller.attackDelay == false)
        {
            Attack(controller);
        }
        else if (controller.attackDelay == true)
        {
            attackDelay(controller);
        }
    }
}
