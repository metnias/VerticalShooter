using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton GameManager
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    /// <summary>
    /// Returns GameManager Singleton Instance
    /// </summary>
    public static GameManager Instance() => instance;

    // Gameplay objects
    public GameObject playerPrefab; // player prefab for respawn
    public Transform[] spawnPoints; // enemy spawn points (1: boss spawn)
    public Pool_Manager[] enemyPools; // enemy pools
    public GameObject explosionPrefab; // explosion prefab
    public GameObject boomPrefab; // boom prefab
    public Pool_Manager[] itemPools; // item pools
    public GameObject bossObject; // boss gameobject
    public Camera_Shake camShake; // camera shake script instance

    // GUI
    public Image[] lifeImages; // life image 
    public Text boomText; // boom counter
    public Text coinText; // coin counter
    public GameObject panelGameover; // gameover panel

    private float enemySpawnDelay = 3f; // enemy spawn delay
    private float enemySpawnCooldown = -3f; // enemy spawn cooldown
    [SerializeField]
    private int bossCountdown = 20; // enemy spawn left until boss battle
    internal static int difficulty = 0; // difficulty which increases after each boss battle

    private int numLife = 3; // number of player life
    private int numBoom = 1; // number of boom left
    private int numCoin = 0; // number of coin collected (score)

    public void AddCoin(int num)
    { numCoin += num; UpdateUI(); }
    public void AddBoom(int num)
    {
        if (numBoom + num <= 9) numBoom += num;
        else
        {
            numBoom += num;
            int o = numBoom - 9;
            numBoom = 9;
            AddCoin(2 * o);
        }
        UpdateUI();
    }
    /// <summary>
    /// Try using boom. 
    /// </summary>
    /// <returns>Whether boom is left and used</returns>
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
            difficulty = 0; // reset difficulty to 0
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
        enemySpawnCooldown = -100000f; // don't spawn normal enemy
    }

    internal void BossDie()
    {
        ClearBullets();
        bossCountdown = 20;
        enemySpawnCooldown = -10f; // break time before the next stage
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
        panelGameover.SetActive(true);
    }

    public static void RestartGame()
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
        ShakeCam(1f);
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

    public static void ShakeCam(float shake)
        => instance.camShake.SetShake(shake);
}
