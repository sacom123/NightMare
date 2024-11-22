using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("EnemyAi/Decision/CanAttackDelay"))]
public class CanAttackDelayDecision : Decision
{
   private bool AttackDelay(StateController controller)
    {
        if(controller.AttackStateDelay == true)
        {
            return true;
        }
        return false;
    }
    public override bool Decide(StateController controller)
    {
        return AttackDelay(controller);
    }
}
