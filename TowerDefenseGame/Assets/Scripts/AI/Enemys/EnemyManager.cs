using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    /* The Enemy Manager will handle the Spawning of Enemies At each point.
     *  - Spawn Enemies
     *  - Manage Wave Count
     *  - Eventually will spawn different types based on waves count.
     *  - Manage Rate at which enemies Spawn.
     * 
     */

    public static EnemyManager Instance { get; private set; }

    private List<Vector3> enemySpawnPos;
    private int spawned;
    
    public Enemy[] Enemies;                     //Going to pick them randomly from this array
    public float spawnDelay = 1.5f;             //Time between each spawn
    public int maxSpawn = 60;                   //maximum number of enemies that will spawn.

    private Coroutine spawnRoutine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void BeginSpawningEnemies()
    {
        enemySpawnPos = new List<Vector3>();
        enemySpawnPos = GameManager.Instance.enemySpawnPos;

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        if (maxSpawn <= 0) yield break;
        Debug.Log("Ola, begin spawning in ze loop");
        spawned = 0;


        while(spawned <= maxSpawn)
        {
            Enemy enemy = Enemies[UnityEngine.Random.Range(0, Enemies.Length)];

            Debug.Log($"in while loop: Enemy spawn is null? {enemySpawnPos == null} and enemySpawn Count  = {enemySpawnPos.Count}");
            foreach (var spawn in enemySpawnPos)
            {
                Debug.Log("in the foreach loop.");
                GameObject go = Instantiate(enemy.enemyPrefab, spawn, Quaternion.identity);
                go.GetComponent<NavMeshAgent>().speed = enemy.speed;
                go.GetComponent<EnemyBehaviour>().BeginTracking(enemy);
                spawned++;

                Debug.Log("spawned: " + spawned);
            }

            yield return new WaitForSeconds(spawnDelay);    
            
        }

        spawnRoutine = null;
    }

}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public float attackDmg;
    public float health;
    public float speed;

}
