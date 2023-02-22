using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance() => instance;

    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public Pool_Manager[] enemyPools;
    public GameObject explosionPrefab;
    public GameObject boomPrefab;
    public Pool_Manager[] itemPools;
    public GameObject bossObject;

    public Image[] lifeImages;
    public Text boomText;
    public Text coinText;

    private float enemySpawnDelay = 3f;
    private float enemySpawnCooldown = -3f;
    [SerializeField]
    private int bossCountdown = 20;
    internal static int difficulty = 0;

    private int numLife = 3;
    private int numBoom = 1;
    private int numCoin = 0;

    public void AddCoin(int num)
    { numCoin += num; UpdateUI(); }
    public void AddBoom(int num)
    { numBoom += num; UpdateUI(); }
    public bool UseBoom()
    {
        if (numBoom > 0) { numBoom--; UpdateUI(); return true; }
        return false;
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            difficulty = 0;
            UpdateUI();
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void UpdateUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
            lifeImages[i].enabled = i + 1 <= numLife;
        boomText.text = numBoom.ToString();
        coinText.text = numCoin.ToString();
    }

    private void Update()
    {
        enemySpawnCooldown += Time.deltaTime;
        if (bossCountdown < 1)
        {
            if (enemySpawnCooldown > 6f) SpawnBoss();
            return;
        }
        if (enemySpawnCooldown > enemySpawnDelay)
        {
            bossCountdown--;
            SpawnEnemy();
            enemySpawnCooldown = 0f;
            enemySpawnDelay = Random.Range(1f, 4f);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPools[Random.Range(0, enemyPools.Length)].TryDequeue(out var enemy))
        {
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }

    private void SpawnBoss()
    {
        if (bossObject.activeSelf) return;
        bossObject.SetActive(true);
        bossObject.transform.position = spawnPoints[1].position;
        enemySpawnCooldown = -100000f;
    }

    internal void BossDie()
    {
        ClearBullets();
        bossCountdown = 20;
        enemySpawnCooldown = -10f;
        difficulty++;
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
        UpdateUI();
        Instantiate(playerPrefab, new Vector3(0f, -7f, 0f), Quaternion.identity);
    }

    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        ClearBullets();
        return boom;
    }

    public static void ClearBullets()
    {
        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (var bullet in bullets)
        {
            var bang = SpawnExplosion(bullet.transform.position);
            bang.transform.localScale = Vector3.one * 0.5f; // half sized
            Destroy(bullet);
        }
    }

    public static GameObject SpawnItem(Vector3 pos, ItemType type)
    {
        if (instance.itemPools[(int)type].TryDequeue(out var item))
        {
            item.transform.position = pos;
            return item;
        }
        return null;
    }
}
