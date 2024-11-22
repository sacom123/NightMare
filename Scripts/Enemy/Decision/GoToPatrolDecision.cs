using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NM;

[CreateAssetMenu(menuName = ("EnemyAi/Decision/GoToPatrol"))]

public class GoToPatrolDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if (controller.waitTime >= controller.MaxWaitTime)
        {
            return true;
        }
        return false;
    }
}
