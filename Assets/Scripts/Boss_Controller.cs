using UnityEngine;
using static UnityEngine.Mathf;

public class Boss_Controller : Enemy_Controller
{
    public Pool_Manager bulletPool;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        isBoss = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        dying = false;
        curPattern = 1;
        patternFire = 8;
        fireDelay = 0.5f;
    }

    protected override void Update()
    {
        base.Update();
        anim.SetBool("Hit", hitShow > 0f);
        if (dying)
        {
            hitShow = 1f;
            bangCooldown -= Time.deltaTime;
            if (bangCooldown <= 0f)
            {
                bangCooldown = 0.3f;
                var bang = GameManager.SpawnExplosion(transform.position +
                    new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f));
                bang.GetComponent<SpriteRenderer>().sortingOrder = 30;
            }
            rBody.velocity = Vector2.zero;
            return;
        }

        Vector3 vel = new Vector3(0f, 3.5f - transform.position.y, 0f).normalized;
        if (player != null)
        {
            vel.x = Clamp(player.transform.position.x - transform.position.x, -1f, 1f);
            vel = Vector3.ClampMagnitude(vel, 1f);
        }
        rBody.velocity = vel * 0.5f;
    }

    protected override void Fire()
    {
        if (dying) return;
        if (player == null) return;

        if (patternFire < 1)
        {
            curPattern = Random.Range(0, GameManager.difficulty + 2);
            if (curPattern > 3) curPattern = 3;

            switch (curPattern)
            {
                case 0:
                    patternFire = Random.Range(5, 15);
                    fireDelay = 0.8f;
                    break;
                case 1:
                    patternFire = Random.Range(6, 10);
                    fireDelay = 0.5f;
                    break;
                case 2:
                    patternFire = Random.Range(3, 8);
                    fireDelay = 0.8f;
                    break;
                case 3:
                    patternFire = Random.Range(5, 10);
                    fireDelay = 1.2f;
                    break;
            }
            return; // break time
        }
        switch (curPattern)
        {
            default:
            case 0: FirePattern0(); break;
            case 1: FirePattern1(); break;
            case 2: FirePattern2(); break;
            case 3: FirePattern3(); break;

        }
        patternFire--;
    }

    private int patternFire = 0;
    private int curPattern = 0;

    /// <summary>
    /// 36 tracking fire
    /// </summary>
    private void FirePattern0()
    {
        float angle = Atan2(player.transform.position.y - transform.position.y,
            player.transform.position.x - transform.position.x) * Rad2Deg;

        const float SPEED = 2f;
        const int COUNT = 36;

        for (int i = 0; i < COUNT; i++)
        {
            if (bulletPool.TryDequeue(out var bullet))
            {
                float a = angle + (360f / COUNT) * i;
                if (a > 180f) a -= 360f;

                Vector2 dir = new(Cos(a * Deg2Rad), Sin(a * Deg2Rad));
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// angled random fire
    /// </summary>
    private void FirePattern1()
    {
        float angle = Random.Range(-140f, -60f); 

        const float SPEED = 4f;
        const int COUNT = 10;

        for (int i = 0; i < COUNT; i++)
        {
            if (bulletPool.TryDequeue(out var bullet))
            {
                float a = angle + (20f / COUNT) * i;

                Vector2 dir = new(Cos(a * Deg2Rad), Sin(a * Deg2Rad));
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// angled focus fire
    /// </summary>
    private void FirePattern2()
    {
        float angle = Atan2(player.transform.position.y - transform.position.y,
            player.transform.position.x - transform.position.x) * Rad2Deg + Random.Range(-15f, -5f);

        const float SPEED = 4f;
        const int COUNT = 10;

        for (int i = 0; i < COUNT; i++)
        {
            if (bulletPool.TryDequeue(out var bullet))
            {
                float a = angle + (20f / COUNT) * i;

                Vector2 dir = new(Cos(a * Deg2Rad), Sin(a * Deg2Rad));
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);
            }
        }
    }


    /// <summary>
    /// 48 tracking varied fire
    /// </summary>
    private void FirePattern3()
    {
        float angle = Atan2(player.transform.position.y - transform.position.y,
            player.transform.position.x - transform.position.x) * Rad2Deg;

        const float SPEED = 2f;
        const int COUNT = 48;

        for (int i = 0; i < COUNT; i++)
        {
            if (bulletPool.TryDequeue(out var bullet))
            {
                float a = angle + (360f / COUNT) * i;
                if (a > 180f) a -= 360f;

                Vector2 dir = new(Cos(a * Deg2Rad), Sin(a * Deg2Rad));
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED * (i % 2 == 0 ? 2f : 1f), ForceMode2D.Impulse);
            }
        }
    }


    private bool dying = false;
    private float bangCooldown = 0f;

    protected override void Die()
    {
        if (dying) return;
        dying = true; bangCooldown = 0f;
        GameManager.Instance().BossDie();
        Invoke(nameof(Kaboom), 3f);
    }

    private void Kaboom()
    {
        GameManager.SpawnBoom(transform.position);
        gameObject.SetActive(false);
        GameManager.SpawnItem(transform.position, ItemType.Power);
        for (int i = 0; i < 10; i++)
        {
            float a = Random.Range(-180f, 180f) * Deg2Rad;
            Vector2 dir = new(Cos(a), Sin(a));
            var coin = GameManager.SpawnItem(transform.position + (Vector3)dir * 0.3f, ItemType.Coin);
            coin.GetComponent<Rigidbody2D>().velocity = dir * 0.5f;
        }
    }
}