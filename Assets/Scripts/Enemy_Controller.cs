using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    protected Rigidbody2D rBody;

    public Sprite hitSprite;
    private Sprite ownSprite;
    protected SpriteRenderer spr;

    public int maxHealth = 1;
    private int health;
    protected float hitShow = 0f;

    protected GameObject player;
    protected bool isBoss = false;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        ownSprite = spr.sprite;
    }

    protected virtual void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = Mathf.CeilToInt(maxHealth * (1 + (isBoss ? 1f : 0.5f) * GameManager.difficulty));
        isVisible = false;
        fireCooltime = -1f;
        hitShow = 0f;
    }

    protected virtual void Update()
    {
        if (hitShow > 0f)
        {
            spr.sprite = Mathf.Sin(hitShow * 50f) > 0f ? hitSprite : ownSprite;
            hitShow -= Time.deltaTime;
        }
        else spr.sprite = ownSprite;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
    }

    protected float fireDelay = 0.5f;
    protected float fireCooltime = -1f;
    protected bool isVisible = false;

    protected virtual void FixedUpdate()
    {
        if (isVisible)
        {
            if (player == null || player.GetComponent<Player_Controller>().Invulnerable)
            { fireCooltime = 0f; return; }
            fireCooltime += Time.deltaTime;
            if (fireDelay < fireCooltime)
            { fireCooltime = 0f; Fire(); }
        }
        else fireCooltime = -1f;
    }

    protected virtual void Fire()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            isVisible = true; return;
        }
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            var bullet = collision.gameObject.GetComponent<Bullet_Handler>();
            if (!bullet.isBoom) Destroy(collision.gameObject);
            OnHit(bullet.power);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var pc = collision.gameObject.GetComponent<Player_Controller>();
            if (pc.Invulnerable) return;
            pc.Die();
            if (!isBoss) OnHit(health); // instakill
        }
    }

    protected void OnHit(int power)
    {
        if (!isVisible) return;
        health -= power;
        if (health > 0)
        {
            hitShow = 0.15f;
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        GameManager.SpawnExplosion(transform.position);
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
}
