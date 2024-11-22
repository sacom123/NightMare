using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("EnemyAi/Actions/Attack Delay"))]
public class AttackDelayAction : Action
{
    private void Delay(StateController controller)
    {
        controller.attackStateDelay += Time.deltaTime;

        if(controller.attackStateDelay >= controller.classStats.AttackDelay)
        {
            if(controller.currentAttackCount >= controller.variables.ShotsInRounds)
            {
                controller.currentAttackCount = 0;
            }
            controller.AttackStateDelay = false;
            controller.attackStateDelay = 0;
        } 
    }

    public override void Act(StateController controller)
    {
        Delay(controller);
    }
}
