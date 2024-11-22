using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격과 동시에 이동하는 액션, 회전 중일 시 회전을 다 하고
/// Strafing이 일어난다  
/// </summary>
/// 
[CreateAssetMenu(menuName =("EnemyAi/Actions/Focus Move"))]
public class FocusMoveAction : Action
{
    private Vector3 currentDest; //현재 이동하는 목표
    private bool aligned;

    public override void OnReadyAction(StateController controller)
    {
        currentDest = controller.nav.destination;
        controller.focusSight = true;
        aligned = false;
    }


    public override void Act(StateController controller)
    {
        if(!aligned)
        {
            controller.nav.destination = controller.personalTarget;
            controller.nav.speed = 0f;
            if(controller.enemyAnimation.angularSpeed == 0f)
            {
                controller.Strafing = true;
                aligned = true;
                controller.nav.destination = currentDest;
                controller.nav.speed = controller.generalStats.evadeSpeed;
            }
        }
        else
        {

        }
    }
}
