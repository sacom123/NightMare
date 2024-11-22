using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic2PlayTime : MonoBehaviour
{
    public StateController controller;

    private float speed = 40f;
    private ParticleSystem par;
    [HideInInspector] public float distance;
    [HideInInspector] public float lifeTime;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public float lifeTimeCheck;
    [HideInInspector] public int Type;
    private void Start()
    {
        //controller = GameObject.Find("EnemySpawnManager").GetComponent<EnemySpawnManager>().EnemyCount[Type].GetComponent<StateController>();

        par = GetComponent<ParticleSystem>();
        
        startPosition = controller.enemyAnimation.MagicTransform.position;
        
        Vector3 targetPosition = (controller.aimTarget.position - new Vector3(1, 0, 1));
        
        distance = Vector3.Distance(startPosition, targetPosition);
        
        lifeTime = distance / speed;
        
        var mainModule = par.main;
        
        mainModule.startLifetime = (lifeTime - 0.05f);
    }
    public void ParticleLifeTime()
    {
        if (par.time >= 3.9f)
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        ParticleLifeTime();
    }
}
