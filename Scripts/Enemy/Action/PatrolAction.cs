using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NM;


[CreateAssetMenu(menuName = ("EnemyAi/Actions/Patrol"))]
public class PatrolAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.enemyAnimation.AbortPendingAim();
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);
        controller.personalTarget = Vector3.positiveInfinity;
        controller.waitTime = 0;
    }

    private void Patrol(StateController controller)
    {
        if (controller.patrolWayPoints.Count == 0)
        {
            return;
        }
        controller.focusSight = false;
        controller.nav.speed = controller.generalStats.patrolSpeed;

        if (controller.nav.remainingDistance <= controller.nav.stoppingDistance && !controller.nav.pathPending)
        {
            controller.variables.patrolTimer += 0.1f;           
            if (controller.variables.patrolTimer >= controller.generalStats.patrolWaitTime)
            {
                controller.wayPointIndex = (controller.wayPointIndex + 1) % controller.patrolWayPoints.Count;
                controller.variables.patrolTimer = 0f;
            }
        }

        try
        {
            controller.nav.destination = Vector3.zero;
            //controller.nav.SetDestination(controller.patrolWayPoints[controller.wayPointIndex].position);
        }
        catch (UnassignedReferenceException)
        {
            Debug.LogError("웨이 포인트가 없어요 세팅해주세요 ", controller.gameObject);
            controller.patrolWayPoints = new List<Transform>
            {
                controller.transform
            };
            controller.nav.SetDestination(controller.transform.position);
            //controller.nav.destination = controller.transform.position;
        }
    }

    public override void Act(StateController controller)
    {
        Patrol(controller);
    }
}
