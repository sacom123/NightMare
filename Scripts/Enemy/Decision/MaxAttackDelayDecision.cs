using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName =("EnemyAi/Decision/MaxAttackDelay"))]
public class MaxAttackDelayDecision : Decision
{
    private bool AttackDelayCount(StateController controller)
    {
        if (controller.currentAttackCount >= controller.MaximumAttackCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override bool Decide(StateController controller)
    {
        return AttackDelayCount(controller);
    }
}
