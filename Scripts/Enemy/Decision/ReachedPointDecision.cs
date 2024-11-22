using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// navMeshAgent ���� ���� �Ÿ��� ���ߴ� ������ ������ �� ���� �ʾҰų�,  ��θ� �˻� ���� �ƴ� ��� true
/// </summary>
[CreateAssetMenu(menuName = ("EnemyAi/Decision/Reached Point"))]

public class ReachedPointDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if(Application.isPlaying == false)
        {
            return false;
        }
        if(controller.nav.remainingDistance <= controller.nav.stoppingDistance && !controller.nav.pathPending)
        { 
            return true;
        }
        else
        {
            return false;
        }
    }
}
