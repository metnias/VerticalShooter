using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance() => instance;

    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    public float enemySpawnDelay = 3f;
    private float enemySpawnCooldown = -5f;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        enemySpawnCooldown += Time.deltaTime;
        if(enemySpawnCooldown > enemySpawnDelay)
        {
            SpawnEnemy();
            enemySpawnCooldown = 0f;
            enemySpawnDelay = Random.Range(1f, 4f);
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
            spawnPoints[Random.Range(0, spawnPoints.Length)].position,
            Quaternion.identity);

    }
}
