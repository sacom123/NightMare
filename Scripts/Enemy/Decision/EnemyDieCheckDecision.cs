using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAi/Decision/PlayerEnable")]
public class EnemyDieCheckDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        if(controller.enemyhelth.enemyIsDead == true)
        {
            return true;
        }
        return false;
    }
}
