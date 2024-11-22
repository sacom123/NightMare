using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 한 방에 6마리 정도의 Enemy 생성
/// Random 값을 받아와서 Statecontrolloer에 삽입
/// spawn point 배열 생성
/// Spawn point에 미리 만들어둔 Enemy Spawn PreFab이나 그런거 넣어준다
/// </summary>

public class EnemySpawnManager : MonoBehaviour
{

    public Transform[] SpawnPoint;
    public GameObject Enemy;
    public GameObject[] EnemyCount;
    //[HideInInspector] public int EnemyType;
    [HideInInspector] public int[] EnemyTypecount;

    public int TotalEnemyType = 5;
    [HideInInspector] private int currentEnemyCount;

    public int TotalEliteType = 1;
    [HideInInspector] private int CurrentEliteCount = 0;

    [HideInInspector] public int RandomSpawnPoint;

    [HideInInspector] public Room Rc;

    public SubRoom Room;

    private void Awake()
    {

        SpawnPoint = new Transform[TotalEliteType + TotalEnemyType];

        for (int j = 0; j < SpawnPoint.Length; j++)
        {
            SpawnPoint[j] = transform.Find("EnemySpawnPoint").GetChild(j);

        }

        EnemyCount = new GameObject[TotalEliteType + TotalEnemyType];
        
        CurrentEliteCount = 0;

        currentEnemyCount = 0;

        EnemyTypecount = new int[TotalEliteType + TotalEnemyType];

        EnemyRandomType();
        setEnemyCount();
    }

    public void setEnemyCount()
    {
        Room.currEnemy = EnemyCount.Length;
        Room.firstEnemySpawn = true;
    }

    public void EnemyRandomType()
    {
        if (!Room.firstEnemySpawn)
        {
            
            for (int i = 0; i < EnemyCount.Length; i++)
            {
                int EnemyType = Random.Range(0, 8);
                Transform randPoint = chooseSpawnPoint();

                if (EnemyCount[i] == null)
                {
                    if (EnemyType < 4 && currentEnemyCount >= TotalEnemyType)
                    {
                        i--;
                    }
                    else if (EnemyType < 4 && currentEnemyCount < TotalEnemyType)
                    {
                        GameObject NormalEnemy = Instantiate(Enemy, chooseSpawnPoint());
                        NormalEnemy.GetComponent<StateController>().classID = (string)System.Enum.GetName(typeof(EnemyClass), EnemyType);
                        NormalEnemy.GetComponent<StateController>().ChangeMesh(EnemyType);
                        NormalEnemy.GetComponent<StateController>().EnemyTypeint = i;
                        NormalEnemy.name = "Nomal " + EnemyType;
                        NormalEnemy.transform.localPosition = randPoint.localPosition;
                        
                        EnemyCount[i] = NormalEnemy;

                        EnemyTypecount[i] = EnemyType;

                        currentEnemyCount++;
                    }

                    //Elite Type Enemy
                    if (EnemyType >= 4 && EnemyType < 8 && CurrentEliteCount >= TotalEliteType)
                    {
                        i--;
                    }
                    //Elite
                    else if (EnemyType >= 4 && EnemyType < 8 && CurrentEliteCount < TotalEliteType)
                    {
                        GameObject EliteEnemy = Instantiate(Enemy, chooseSpawnPoint());
                        EliteEnemy.GetComponent<StateController>().classID = (string)System.Enum.GetName(typeof(EnemyClass), EnemyType);
                        EliteEnemy.GetComponent<StateController>().EnemyTypeint = i;
                        EliteEnemy.GetComponent<StateController>().ChangeMesh(EnemyType);
                        EliteEnemy.name = "Elite " + EnemyType;
                        EliteEnemy.transform.localPosition = randPoint.localPosition;
                        
                        EnemyCount[i] = EliteEnemy;

                        EnemyTypecount[i] = EnemyType;

                        CurrentEliteCount++;
                    }
                }
            }


        }
    }

    public Transform chooseSpawnPoint()
    {
        RandomSpawnPoint = Random.Range(0, SpawnPoint.Length);

        return SpawnPoint[RandomSpawnPoint];
    }


}