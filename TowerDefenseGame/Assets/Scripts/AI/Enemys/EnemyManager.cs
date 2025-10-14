using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using static EnemyManager;
using static UnityEngine.InputManagerEntry;

public class EnemyManager : MonoBehaviour
{
    /* The Enemy Manager will handle spawning per wave
     *  - Waves loop forever (or until maxTotalSpawn > 0 and reached)
     *  - Enemies per wave scale up with wave number
     *  - Difficulty increases every N waves (default: 2)
     *  - WaveType changes when difficulty increases
     *  - WaveType controls enemy composition
     */

    public static EnemyManager Instance { get; private set; }

    private List<Vector3> enemySpawnPos;
    private int spawned;

    [Header("Enemy Catalog (ordered: weakest -> strongest)")]
    public Enemy[] Enemies;                     //Going to pick them randomly from this array

    [Header("Timing")]
    [Tooltip("Time between individual spawns")]
    public float spawnDelay = 1.5f;             //Time between each spawn
    [Tooltip("Delay between waves")]
    public float waveDelay = 10f;               //Time between each Wave

    [Header("Wave Sizing")]
    [Tooltip("Base enemies in wave 1")]
    public int baseEnemiesPerWave = 30;
    private int waveEnemyCount;
    public int resourceBonus = 500;
    public List<WaveMix> waveMixList;

    [Tooltip("Linear increment per wave (additive). Set to 0 if you prefer exponential growth.")]
    public int enemiesPerWaveIncrement = 5;

    [Header("Difficulty")]
    private int difficultyLevel = 0;
    [Tooltip("Increase difficulty every N waves")]
    public int wavesPerDifficulty = 2;


    public enum WaveType { Easy, Test, Spike, Relax, Bonus }
    private WaveType currentWaveType = WaveType.Easy;


    //RunTime STUFF
    private int waveCounter = -1;

    private Coroutine waveLoopRoutine;
    private Coroutine SpawningRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
        }
    }


    public void BeginSpawningEnemies()
    {
        enemySpawnPos = new List<Vector3>();
        enemySpawnPos = GameManager.Instance.enemySpawnPos;

        waveLoopRoutine = StartCoroutine(WaveLoop());
    }

    private void SetUpWave()
    {
        waveCounter++;       

        if(difficultyLevel >= Enum.GetNames(typeof(WaveType)).Length)
        {
            difficultyLevel = 0;
        }

        if(waveCounter > 1 && waveCounter % wavesPerDifficulty == 0)
        {
            difficultyLevel++;
        }

        currentWaveType = (WaveType)difficultyLevel;
        waveEnemyCount = baseEnemiesPerWave + (waveCounter * enemiesPerWaveIncrement);
    }


    private IEnumerator WaveLoop()
    {
        SetUpWave();

        SpawningRoutine = StartCoroutine(SpawnLoop());      //Spawning Enemies Takes Place Here <<<<<<<<<<<

        yield return new WaitForSeconds(waveDelay);

    }

    private void EndOfWave()
    {

        var waveIncome = resourceBonus;
        switch (currentWaveType)
        {
            case WaveType.Spike: waveIncome += 500; break;
            case WaveType.Bonus: waveIncome += 500; break;
            default: break;
        }

        EconomyManager.Instance.EarnResorces(waveIncome);
        BeginSpawningEnemies();
    }


    private IEnumerator SpawnLoop()
    {
        if (waveEnemyCount <= 0) yield break;
        spawned = 0;


        if(currentWaveType == WaveType.Bonus)
        {
            int[] spawnAmount = GetNumberOfSpawnsPerType();
            int i = 0;

            while (spawned < spawnAmount.Sum())
            {
                Enemy enemy = Enemies[i];

                for (int j = 0; j < spawnAmount[i] - (spawnAmount[i] * 0.5); j++)
                {
                    foreach (var spawn in enemySpawnPos)
                    {
                        GameObject go = Instantiate(enemy.enemyPrefab, spawn, Quaternion.identity);
                        go.GetComponent<NavMeshAgent>().speed = enemy.speed;
                        go.GetComponent<EnemyBehaviour>().BeginTracking(enemy);

                    }
                    spawned++;
                    yield return new WaitForSeconds(spawnDelay);
                }

                i++;

            }
        }
        else
        {
            int[] spawnAmount = GetNumberOfSpawnsPerType();
            int i = 0;
            //                                  7                                   6                                   1               //Numbers are here for me to think
            //Debug.Log($"spawnAmount is {spawnAmount.Sum()} with goblin being {spawnAmount[0]} and Troll being {spawnAmount[1]}");   

            //>>>>>>>>>>>>>>>>  DO NOT TOUCH LOOP     <<<<<<<<<<<<<<<<
            while (spawned < spawnAmount.Sum())
            {
                Enemy enemy = Enemies[i];

                for (int j = 0; j < spawnAmount[i]; j++)
                {
                    foreach (var spawn in enemySpawnPos)
                    {
                        GameObject go = Instantiate(enemy.enemyPrefab, spawn, Quaternion.identity);
                        go.GetComponent<NavMeshAgent>().speed = enemy.speed;
                        go.GetComponent<EnemyBehaviour>().BeginTracking(enemy);                                                
                    }

                    spawned++;
                    yield return new WaitForSeconds(spawnDelay);
                }

                Debug.Log($"Ola, Spawned is {spawned} and the Enemies index is currently {i}");
                i++;
            }
        }

        yield return new WaitForSeconds(waveDelay);

        Debug.Log("FinishedSpawning");
        EndOfWave();

    }

    private int[] GetNumberOfSpawnsPerType()
    {
        var waveMix = waveMixList[(int)currentWaveType];
        int[] spawnNum = new int[waveMix.percentageMix.Count];

        for(int i = 0; i < waveMix.percentageMix.Count; i++)
        {
            //Get the Number of Shits to Spawn
            var p = waveEnemyCount * (waveMix.percentageMix[i] / 100);
            spawnNum[i] = ((int)p);
        }

        return spawnNum;
    }



}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public string enemyTypeName;
    public float attackDmg;
    public float attackRadius;
    public float health;
    public float speed;

}


[System.Serializable]
public class WaveMix
{
    public WaveType waveType;
    public List<float> percentageMix = new List<float>();
}



//OLD CODE

//public void BeginSpawningEnemies()
//{
//    enemySpawnPos = new List<Vector3>();
//    enemySpawnPos = GameManager.Instance.enemySpawnPos;

//    waveLoopRoutine = StartCoroutine(SpawnLoop());
//}



//private IEnumerator SpawnLoop()
//{
//    if (waveEnemyCount <= 0) yield break;
//    spawned = 0;

//    while (spawned <= waveEnemyCount)
//    {
//        Enemy enemy = Enemies[UnityEngine.Random.Range(0, Enemies.Length)];

//        foreach (var spawn in enemySpawnPos)
//        {
//            GameObject go = Instantiate(enemy.enemyPrefab, spawn, Quaternion.identity);
//            go.GetComponent<NavMeshAgent>().speed = enemy.speed;
//            go.GetComponent<EnemyBehaviour>().BeginTracking(enemy);
//            spawned++;

//        }

//        yield return new WaitForSeconds(spawnDelay);

//    }

//    waveLoopRoutine = null;
//    waveCounter += 1;
//}

//while (spawned <= waveEnemyCount)
//{
//    Enemy enemy = Enemies[UnityEngine.Random.Range(0, Enemies.Length)];

//    foreach (var spawn in enemySpawnPos)
//    {
//        GameObject go = Instantiate(enemy.enemyPrefab, spawn, Quaternion.identity);
//        go.GetComponent<NavMeshAgent>().speed = enemy.speed;
//        go.GetComponent<EnemyBehaviour>().BeginTracking(enemy);
//        spawned++;

//    }

//    yield return new WaitForSeconds(spawnDelay);

//}