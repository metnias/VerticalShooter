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
    public GameObject boomPrefab;

    private float enemySpawnDelay = 3f;
    private float enemySpawnCooldown = -5f;

    private int numLife = 3;
    private int numBoom = 1;
    private int numCoin = 0;

    public void AddCoin(int num) => numCoin += num;
    public void AddBoom(int num) => numBoom += num;

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
        Invoke(nameof(RevivePlayer), 2f);
    }

    private void RevivePlayer()
    {
        if (numLife < 1)
        {
            GameOver(); return;
        }
        numLife--;
        Instantiate(playerPrefab, new Vector3(0f, -7f, 0f), Quaternion.identity);
    }

    private void GameOver()
    {

    }

    public static GameObject SpawnExplosion(Vector3 pos)
    {
        var bang = Instantiate(instance.explosionPrefab, pos, Quaternion.identity);
        Destroy(bang, 0.34f);
        return bang;
    }

    public static GameObject SpawnBoom(Vector3 pos)
    {
        var boom = Instantiate(instance.boomPrefab, pos, Quaternion.identity);
        Destroy(boom, 0.5f);

        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (var bullet in bullets)
        {
            var bang = SpawnExplosion(bullet.transform.position);
            bang.transform.localScale = Vector3.one * 0.5f; // half sized
            Destroy(bullet);
        }
        return boom;
    }
}
