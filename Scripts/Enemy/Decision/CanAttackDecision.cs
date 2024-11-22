using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("EnemyAi/Decision/CanAttack"))]
public class CanAttackDecision : Decision
{
    public bool CanAttack(StateController controller)
    {
        float distance = (controller.aimTarget.transform.position - controller.transform.position).sqrMagnitude;
        if (distance > Mathf.Pow(controller.classStats.Range,2))
        {
            return false;
        }
        else if(distance <= Mathf.Pow(controller.classStats.Range, 2))
        {
            return true;
        }
        return false;
    }
    public override bool Decide(StateController controller)
    {
        return CanAttack(controller);
    }
}
