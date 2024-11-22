using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.aiActive = false;
        controller.nav.speed = 0;
        //controller.nav.ResetPath();
    }
    private void DieAction(StateController controller, float time)
    {
        controller.enemyAnimation.anim.SetBool("Die", true);
        Destroy(controller.gameObject, time);
    }
    public override void Act(StateController controller)
    {
        DieAction(controller, 3.0f);
    }
}
