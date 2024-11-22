using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NM;

[CreateAssetMenu(menuName = ("EnemyAi/Actions/StopWaitAction"))]
public class StopWaitAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.enemyAnimation.AbortPendingAim();
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);
        controller.personalTarget = Vector3.positiveInfinity;
        controller.waitTime = 0;
    }

    private void Wait(StateController controller)
    {
        if (controller.nav.remainingDistance <= controller.nav.stoppingDistance  || !controller.nav.pathPending )
        {
            controller.waitTime += 0.1f;
        }

    }

    public override void Act(StateController controller)
    {
        Wait(controller);
    }
}
