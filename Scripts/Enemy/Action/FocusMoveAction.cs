using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ݰ� ���ÿ� �̵��ϴ� �׼�, ȸ�� ���� �� ȸ���� �� �ϰ�
/// Strafing�� �Ͼ��  
/// </summary>
/// 
[CreateAssetMenu(menuName =("EnemyAi/Actions/Focus Move"))]
public class FocusMoveAction : Action
{
    private Vector3 currentDest; //���� �̵��ϴ� ��ǥ
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
