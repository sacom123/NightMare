using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="EnemyAi/Decision/CanGoAttack")]
public class CanGoAttackDecision : Decision
{
   private bool AttackCount(StateController controller)
    {

        if(controller.currentAttackCount == 0 && controller.attackDelay == false)
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
        return AttackCount(controller);
    }
}
