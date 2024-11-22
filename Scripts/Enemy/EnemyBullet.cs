using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBullet : MonoBehaviour
{
    //Player transform�� �޾ƿ� ��Ʈ�ѷ�
    public StateController controller;

    [HideInInspector] public int Type;
    //�� ��ġ�� ��� �ִ°�
    [HideInInspector]public Transform Target;
    //��ƼŬ �ý���
    [HideInInspector]private ParticleSystem particle;


    //�浹 Ȯ�� ����
    private bool hasCollided = false;

    //�߻� �ǰ� ���� �ڿ� ���� ���ΰ�
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
