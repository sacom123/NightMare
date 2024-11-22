using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySwordAttack : MonoBehaviour
{
    [HideInInspector] public StateController controller;
    [HideInInspector] public Transform target;
    [HideInInspector] public int Type;
    public Image DamageEffect;
    private bool hasCollided = false;
    private ParticleSystem particle;

    void Start()
    {
        target = controller.aimTarget;
        particle = GetComponent<ParticleSystem>();
        
        particle.trigger.AddCollider(GameObject.FindWithTag("Player").GetComponent<CharacterController>());
        particle.trigger.AddCollider(GameObject.FindWithTag("Player").transform);
        
    }

    public void OnParticleCollision(GameObject other)
    {
        Player player = other.GetComponent<Player>();
        if (hasCollided == false)
        {
            if (player != null && player.IsDead == false)
            {
                Debug.Log("Ãæµ¹¶ì");
                player.TakeDamage(controller.classStats.Damaege);
                hasCollided = true;
            }
        }
    }
}
