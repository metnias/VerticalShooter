using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance() => instance;

    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public GameObject explosionPrefab;

    private float enemySpawnDelay = 3f;
    private float enemySpawnCooldown = -5f;
    private int life = 3;

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

    public void PlayerDie()
    {
        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (var bullet in bullets) Destroy(bullet);
        Invoke(nameof(RevivePlayer), 2f);
    }

    private void RevivePlayer()
    {
        if (life < 1)
        {
            GameOver(); return;
        }
        life--;
        Instantiate(playerPrefab, new Vector3(0f, -7f, 0f), Quaternion.identity);
    }

    private void GameOver()
    {

    }

    public static void SpawnExplosion(Vector3 pos)
    {
        var boom = Instantiate(instance.explosionPrefab, pos, Quaternion.identity);
        Destroy(boom, 0.34f);
    }
}
