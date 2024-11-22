using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 시야각 (1/2)사이에 있다면
/// 
/// </summary>
[CreateAssetMenu(menuName = ("EnemyAi/Decision/LookDecision"))]
public class LookDecision : Decision
{
    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[]targetInRadius)
    {
        if(hasTarget)
        {
            //플레이어 위치
            Vector3 target = targetInRadius[0].transform.position;

            Vector3 dirToTarget = target - controller.transform.position;
            bool inFOVCondition = (Vector3.Angle(controller.transform.forward, dirToTarget) < controller.viewAngle / 2);

            if(inFOVCondition && !controller.aimTarget.root.GetComponent<Player>().IsDead)
            {
                controller.targetInSight = true;
                controller.personalTarget = controller.aimTarget.position;
                return true;
            }
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        controller.targetInSight = false;
        return Decision.ChechkTargetInRadius(controller, controller.viewAngle, MyHandleTargets);
    }
}
