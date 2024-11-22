using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("EnemyAi/Decision/StopWait"))]
public class StopWaitDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if (Application.isPlaying == false)
        {
            return false;
        }
        if (controller.nav.remainingDistance <= controller.nav.stoppingDistance || !controller.nav.pathPending)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
