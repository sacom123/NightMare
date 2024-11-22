using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBullet : MonoBehaviour
{
    //Player transform을 받아올 컨트롤러
    public StateController controller;

    [HideInInspector] public int Type;
    //적 위치가 어디에 있는가
    [HideInInspector]public Transform Target;
    //파티클 시스템
    [HideInInspector]private ParticleSystem particle;


    //충돌 확인 여부
    private bool hasCollided = false;

    //발사 되고 몇초 뒤에 지울 것인가
    public float lifeTime = 3f;

    public GameObject parentTransform;

    void Start()
    {
        if(transform.tag == "Magic2")
        {
            controller = parentTransform.GetComponent<Magic2PlayTime>().controller;
        }
        else
        {
            parentTransform = gameObject;
        }
        Target = controller.aimTarget;

        particle = GetComponent<ParticleSystem>();

        particle.trigger.AddCollider(GameObject.FindWithTag("Player").GetComponent<CharacterController>());
        particle.trigger.AddCollider(GameObject.FindWithTag("Player").transform);

        hasCollided = false;
    }


    public void OnParticleCollision(GameObject other)
    {
        int layer = LayerMask.NameToLayer("Bound");

        Player player = other.GetComponent<Player>();
                
        if(hasCollided == false)
        {
            Debug.Log("parentTransform : " + parentTransform);
            parentTransform.GetComponent<ParticleSystem>().Stop();

            if (player != null && player.IsDead == false && hasCollided == false)
            {                
                player.TakeDamage(controller.classStats.Damaege);
                hasCollided = true;
            }
            if (other.layer == layer)
            {
                parentTransform.GetComponent<ParticleSystem>().Stop();
                GetComponent<ParticleSystem>().Stop();
            }
        }
    }

}
