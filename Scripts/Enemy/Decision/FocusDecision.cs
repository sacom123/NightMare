using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("EnemyAi/Decision/FocisDecision"))]
public class FocusDecision : Decision
{
    public enum Sense
    {
        NEAR,
        PERCEPTION,
        VIEW,
    }
    [Tooltip("어떤 크기로 위험 요소 감지를 하겠습니까?")]
    public Sense sense;

    private float radius;

    public override void OnEnableDecision(StateController controller)
    {
        switch(sense)
        {
            case Sense.NEAR:
                radius = controller.nearRadius;
                break;
            case Sense.PERCEPTION:
                radius = controller.perceptionRadius;
                break;
            case Sense.VIEW:
                radius = controller.viewRadius;
                break;
            default:
                radius = controller.nearRadius;
                break;
        }
    }

    private bool MyHandleTargets(StateController controller,bool hasTarget,Collider[]targetsInHearRadius)
    {
        //타겟이 존재
        if(hasTarget)
        {
            controller.targetInSight = true;
            controller.personalTarget = controller.aimTarget.position;
            return true;
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        return ( sense != Sense.NEAR || Decision.ChechkTargetInRadius(controller, radius, MyHandleTargets));
    }


}
