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

    //기본적으로 시행해야할 함수들
    public override void OnReadyAction(StateController controller)
    {

        controller.enemyAnimation.anim.SetFloat("AttackSpeed", 1f);

        //앉아있었으면 초기화
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);

        //Enemy의 navmesh agent의 speed를 0으로 고정
        controller.nav.speed = 0f;

        controller.enemyAnimation.ActivatePendingAim(); //조준 대기, 시야 + 범위안에 들어오면 공격 가능
        
        effectId = controller.effecid;

    }
    //데미지를 가하는 함수
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

    //근접 타입1의 공격 애니메이션과 파티클 시행
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
        destroyDelayed.DelayTime = 2f; //공격 속도에 맞춰서 추후에 수정
        controller.attackDelay = true;
        EnemyDealing(controller, target, organic);
    }

    //파이어볼 발사
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

    
    // 2번째 타입의 발사 추가
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

    // 받아온 Vector3 값들과 bool, transform들을 기반으로 effectID를 검사해서 해당 타입에 해당하는 case로 가서 공격을 시행
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

    // Ray Cast를 사용해서 공격할 대상 위치 정확히 파악
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


    // 공격이 가능한지에 대한 여부 확인 EX) 거리, 각도, classStats에서 정한 Range 값 안에 들어 왔는가 
    private bool CanAttack(StateController controller)
    {
        //총을 쏠 수 있는 상태인가, 너무 가깝지 않거나 타이머 제한
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

    // 현재 공격한 횟수와 총 공격할 수 있는 횟수 비교, 게임이 실행중인지, 지금 공격 중인지 아닌지 등 검사
    private void Attack(StateController controller)
    {
        if (controller.currentAttackCount >= controller.variables.ShotsInRounds)
        {
            controller.AttackStateDelay = true;
        }
        //castshot 함수 호출 후 shotTimer를 1 증가
        if (Time.timeScale > 0 && controller.currentAttackCount <= controller.variables.ShotsInRounds && controller.isAttacking == false && controller.variables.shotTimer < 1)
        {
            CastShot(controller);
            controller.isAttacking = true;
            controller.variables.shotTimer += 1;
        }
        //공격이 실행 됐을 경우 CurrentShots 증가 시키고 shotTimer를 0으로 초기화
        else if (controller.variables.shotTimer >= 1)
        {
            controller.variables.shotTimer = 0;
            return;
        }
    }

    //공격중에 딜레이를 걸어줌 걸어 주지 않으면 너무 빠르게 공격해서 애니메이션이 망가지고 피가 빠르게 빠짐, 공격 애니메이션이 나오지 않았는데도 피가 깎인다.
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

    //실질 적인 행동을 하는 함수 update 와 같은 역할을 한다
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
