using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NM;

[CreateAssetMenu(menuName = ("EnemyAi/Actions/PlayerFollow"))]
public class PlayerFollowAction : Action
{
    private float distance;
    public AttackAction attack;
    public override void OnReadyAction(StateController controller)
    {
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);
        controller.enemyAnimation.anim.speed = controller.originAnimSpeed;
        controller.personalTarget = controller.aimTarget.position;
        //controller.nav.destination = Vector3.zero;
        controller.waitTime = 0;
        controller.nav.speed = controller.generalStats.chaseSpeed;
    }

    //public void castMagicInFollow(StateController controller)
    //{
    //    if(CastEffectcount == 0)
    //    {
    //        attack.MakeEffectMagicCast(controller);
    //    }
    //}
    public void Follow(StateController controller)
    {
        distance = (controller.aimTarget.transform.position - controller.enemyAnimation.transform.position).sqrMagnitude;

        if (controller.aimTarget.root.GetComponent<Player>().IsDead)
        {
            return;
        }

        if(distance > controller.perceptionRadius)
        {
            controller.nav.speed = controller.generalStats.chaseSpeed;
        }
        else if(distance <= controller.perceptionRadius)
        {
            controller.nav.speed = controller.generalStats.chaseSpeed / 2;
        }
        
        try
        {
            controller.personalTarget = controller.aimTarget.position;
            controller.Aiming = true;
            controller.enemyAnimation.transform.LookAt(controller.aimTarget);
            //if((int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType) == 2)
            //{
            //    castMagicInFollow(controller);
            //}
            
            controller.nav.SetDestination(controller.personalTarget);
        }

        catch (UnassignedReferenceException)
        {
            Debug.LogError("플레이어가 존재하지 않아요 ", controller.gameObject);
            controller.patrolWayPoints = new List<Transform>
            {
                controller.transform
            };
            controller.nav.destination = controller.transform.position;
        }
    }
    public override void Act(StateController controller)
    {
          Follow(controller);
    }
}
