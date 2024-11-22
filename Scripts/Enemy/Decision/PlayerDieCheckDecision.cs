using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("EnemyAi/Decision/PlayerEnable"))]
public class PlayerDieCheckDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if(controller.aimTarget.GetComponent<Player>().IsDead == false)
        {
            return controller.aimTarget.root.GetComponent<Player>().IsDead;
        }
            Debug.LogError("����� ���� ������Ʈ Health Bass�� �ٿ��ּ��� " + controller.name, controller.gameObject);
            return controller.aimTarget.root.GetComponent<Player>().IsDead;
    }
}
