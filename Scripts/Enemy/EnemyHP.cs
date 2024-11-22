using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyHP : MonoBehaviour
{
    public float enemyMaxHP = 100f;
    public float currentHP = 0f;
    public bool enemyIsDead = false;
    public bool checkEnemyIsDeadOne = false;
    
    StateController controller;
    public SkinnedMeshRenderer Render;
    private void Awake()
    {
        enemyIsDead = false;
        controller = GetComponent<StateController>();

        

        DOTween.Init();

    }
    void Update()
    {
        if (currentHP <= 0)
        {
            enemyIsDead = true;
        }
        if (enemyIsDead == true && checkEnemyIsDeadOne == false)
        {
            RoomController.Instance.currSubRoom.currEnemy--;
            IsDieAtcion(controller, 3.0f);
            checkEnemyIsDeadOne = true;
        }
    }

    public void HpSet()
    {
        currentHP = enemyMaxHP;
    }
    public void TakeDamage(float damage)
    {
        if(!enemyIsDead)
        {
            currentHP -= damage;
            var seq = DOTween.Sequence();
            seq.Append(Render.material.DOColor(Color.red, 0.5f));
            seq.Play().OnComplete(() =>
            {
                Render.material.DOColor(Color.white, 1f);
            });
        }      
    }

    private void IsDieAtcion(StateController controller, float time)
    {
        controller.aiActive = false;
        controller.enemyAnimation.anim.SetBool("Die", true);
        Destroy(gameObject, time);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            controller.enemyAnimation.anim.SetTrigger("Hit");
        }
    }
}
