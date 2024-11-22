using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NM;
/// <summary>
/// Enemy 의 타입에 따른 Animation들을 지정해주고 실행시켜주는 스크립트
/// 
/// </summary>
public class EnemyAnimation : MonoBehaviour
{
    [HideInInspector]public Animator anim;
    [HideInInspector]public float angularSpeed;
    [HideInInspector]public float currentAimAngleGap;
    [HideInInspector]public Transform effectTransform;
    [HideInInspector]public Transform MagicTransform;
    [HideInInspector] public Transform EffectTransformPernt;
    [HideInInspector] public Transform MagicTransformPernt;

    private StateController controller;
    private NavMeshAgent nav;
    private bool pendingAim; //조준을 기다리는 시간
    [HideInInspector]public Transform hips, spine; // bone transform
    

    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Quaternion lastRotation;
    private float timeCountAim, timeCountGuard;
    private readonly float turnSpeed = 25f; //스트래핑 할때 적의 회전 속도 Strafing trun speed.

    private void Awake()
    {
        //Setup
        controller = GetComponent<StateController>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.updateRotation = false;

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;

        foreach (Rigidbody member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    public void Start()
    {
        anim.SetTrigger(AnimatorKey.ChangeWeapon);
        if (controller.random != 8)
        {
            anim.SetInteger(AnimatorKey.Weapon,
            (int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType));
        }

        if ((int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType) == 0 ||
            (int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType) == 1)
        {
            foreach (Transform child in this.anim.GetBoneTransform(HumanBodyBones.RightHand))
            {
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }
                effectTransform = child.Find("EffectTransform");
                if (effectTransform != null)
                {
                    break;
                }
            }
        }

        if ((int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType) == 2 ||
            (int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeponType) == 3)
        {
            foreach (Transform child in anim.transform.Find("Root"))
            {
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }
                MagicTransform = child.Find("MagicTransform");
                //Debug.Log(MagicTransform);
                if (MagicTransform != null)
                {
                    break;
                }
            }
        }
    }
    void Setup(float speed, float angle,Vector3 strafeDirection)
    {
        angle *= Mathf.Deg2Rad;
        angularSpeed = angle / controller.generalStats.angleResponseTime;

        anim.SetFloat(AnimatorKey.Speed, speed, controller.generalStats.speedDampTime,Time.deltaTime);
        anim.SetFloat(AnimatorKey.AngularSpeed, angularSpeed, controller.generalStats.angularSpeedDampTime, Time.deltaTime);

        anim.SetFloat(AnimatorKey.Horizontal, strafeDirection.x, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(AnimatorKey.Vertical, strafeDirection.y, controller.generalStats.speedDampTime, Time.deltaTime);

    }
    public Transform FindWeaponTransform(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if(child.name == "Weapon")
            {
                return child;
            }
        }
        return null;
    }

    void NavAnimSetUp()
    {
        float speed;
        float angle;
        speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
        if(controller.focusSight)
        {

            Vector3 dest = (controller.personalTarget - transform.position);
            dest.y = 0.0f;
            angle = Vector3.SignedAngle(transform.forward, dest, transform.up);
            if(controller.Strafing)
            {
                dest = dest.normalized;
                Quaternion targetStrafingRotation = Quaternion.LookRotation(dest);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetStrafingRotation, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            if(nav.desiredVelocity == Vector3.zero)
            {
                angle = 0.0f;
            }
            else
            {
                angle = Vector3.SignedAngle(transform.forward, nav.desiredVelocity, transform.up);
            }
        }

        if (!controller.Strafing && Mathf.Abs(angle) < controller.generalStats.angleDeadZone)
        {
            transform.LookAt(transform.position + nav.desiredVelocity);
            angle = 0;
            if (pendingAim && controller.focusSight)
            {
                controller.Aiming = true;
                pendingAim = false;
            }
        }

        //Strafe direction
        Vector3 direction = nav.desiredVelocity;
        direction.y = 0.0f;
        direction = direction.normalized;
        direction = Quaternion.Inverse(transform.rotation) * direction;
        Setup(speed, angle, direction);
    }

    private void Update()
    {
        NavAnimSetUp();
    }

    private void OnAnimatorMove()
    {
        if(Time.timeScale > 0 && Time.deltaTime >0)
        {
            nav.velocity = anim.deltaPosition / Time.deltaTime;
            if(!controller.Strafing)
            {
                transform.rotation = anim.rootRotation;
            }
        }
    }

    private void LateUpdate()
    {
        if (controller.Aiming)
        {
            controller.personalTarget = controller.aimTarget.position;
            Vector3 direction = controller.personalTarget - spine.position;
            if (direction.magnitude < 0.01f || direction.magnitude > 100000.0f)
            {
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipsRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);
            targetRotation *= Quaternion.Euler(0, 0, 0);
            Quaternion frameRotation = Quaternion.Slerp(lastRotation, targetRotation, timeCountAim);
            //엉덩이 뼈 기준 척추 회전이 60도 이하인 경우 계속 조준 가능
            if (Quaternion.Angle(frameRotation, hips.rotation) <= 60.0f)
            {
                spine.rotation = frameRotation;
                timeCountAim += Time.deltaTime;
            }
            else
            {
                if (timeCountAim == 0 && Quaternion.Angle(frameRotation, hips.rotation) > 70.0f)
                {
                    StartCoroutine(controller.UnstuckAim(2f));
                }
                spine.rotation = lastRotation;
                timeCountAim = 0;
            }

            lastRotation = spine.rotation;
            Vector3 target = controller.personalTarget - controller.enemyAnimation.anim.transform.position;
            Vector3 forward = controller.enemyAnimation.anim.transform.forward;
            currentAimAngleGap = Vector3.Angle(target, forward);
            timeCountGuard = 0;
        }
        else
        {
            lastRotation = spine.rotation;
            spine.rotation *= Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.identity, timeCountGuard);
            timeCountGuard += Time.deltaTime;
        }

    }

    public void ActivatePendingAim()
    {
        pendingAim = true;
    }
    public void AbortPendingAim()
    {
        pendingAim = false;
        controller.Aiming = false;
    }

}
