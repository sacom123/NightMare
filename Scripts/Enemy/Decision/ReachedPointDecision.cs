using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// navMeshAgent 에서 남은 거리가 멈추는 도중일 정도로 얼마 남지 않았거나,  경로를 검색 중이 아닌 경우 true
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
