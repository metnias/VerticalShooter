using System;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public string bulletPoolAname = "PoolPBulletA";
    public string bulletPoolBname = "PoolPBulletB";

    private Pool_Manager bulletPoolA;
    private Pool_Manager bulletPoolB;

    public float speed = 4f;
    public int power = 1;
    public float[] bulletDelay;

    private Animator animator;
    private Rigidbody2D rBody;
    private SpriteRenderer spr;

    private const string ANIMX = "DirX";

    private float dirX = 0f;
    private float dirY = 1f;
    private float bulletCooldown = 0f;

    private int borderTouch = 0; // border touch flags with bitmask

    private bool canControl = false;

    [Flags]
    private enum Border
    {
        Top = 1 << 1, Bottom = 1 << 2, Left = 1 << 3, Right = 1 << 4
            // names matched with gameobjects' names
    }

    private float invulnerability; // iframes in seconds
    internal bool Invulnerable => invulnerability > 0f;
    private bool lastBoom = false; // whether boom key is pressed last frame

    private void Start()
    {
        animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        bulletPoolA = GameObject.Find(bulletPoolAname).GetComponent<Pool_Manager>();
        bulletPoolB = GameObject.Find(bulletPoolBname).GetComponent<Pool_Manager>();

        invulnerability = 2f;
        Invoke(nameof(GainControl), 1f);
        GetComponent<CircleCollider2D>().enabled = false;
    }

    private void Update()
    {
        IFrame();
        Move();
        Fire();
        ReloadBullet();
        Boom();
    }

    internal void GainControl()
    {
        canControl = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void Move()
    {
        if (!canControl) return;

        dirX = Input.GetAxisRaw("Horizontal");
        if ((borderTouch & (int)Border.Left) > 0 && dirX < 0f) dirX = 0f;
        else if ((borderTouch & (int)Border.Right) > 0 && dirX > 0f) dirX = 0f;
        // clamping using border flags
        dirY = Input.GetAxisRaw("Vertical");
        if ((borderTouch & (int)Border.Bottom) > 0 && dirY < 0f) dirY = 0f;
        else if ((borderTouch & (int)Border.Top) > 0 && dirY > 0f) dirY = 0f;

        animator.SetInteger(ANIMX, dirX == 0 ? 0 : (int)Mathf.Sign(dirX)); // set animation
    }

    private void Fire()
    {
        if (!canControl) return;
        if (!Input.GetButton("Fire1")) return;
        if (bulletCooldown < bulletDelay[power - 1]) return;
        bulletCooldown = 0f;

        Power();
    }

    private void Power()
    {
        switch (power)
        {
            case 1: // pow 1 bullet
                {
                    if (bulletPoolA.TryDequeue(out var bullet))
                    {
                        bullet.transform.position = transform.position;
                        bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }
                }
                break;
            case 2: // 2 pow 1 bullets
                {
                    if (bulletPoolA.TryDequeue(out var bulletR))
                    {
                        bulletR.transform.position = transform.position + Vector3.right * 0.15f;
                        bulletR.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }

                    if (bulletPoolA.TryDequeue(out var bulletL))
                    {
                        bulletL.transform.position = transform.position + Vector3.left * 0.15f;
                        bulletL.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }
                }
                break;
            default:
            case 3: // 2 pow 1 bullets & 1 pow 2 bullet
                {

                    if (bulletPoolA.TryDequeue(out var bulletR))
                    {
                        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
                        bulletR.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }

                    if (bulletPoolA.TryDequeue(out var bulletL))
                    {
                        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
                        bulletL.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }

                    if (bulletPoolB.TryDequeue(out var bulletB))
                    {
                        bulletB.transform.position = transform.position;
                        bulletB.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                    }
                }
                break;
        }

    }

    private void ReloadBullet()
    {
        bulletCooldown += Time.deltaTime;
    }

    private void IFrame()
    {
        if (invulnerability > 0f)
        {
            invulnerability -= Time.deltaTime;
            spr.enabled = Mathf.Sin(invulnerability * 50f) > 0f; // sprite blinking
        }
        else { invulnerability = 0f; spr.enabled = true; } // sprite on
    }

    private void Boom()
    {
        if (!canControl) return;

        if (!Input.GetButton("Fire2")) { lastBoom = false; return; }
        if (lastBoom) return;
        lastBoom = true;
        if (!GameManager.Instance().UseBoom()) return; // no boom left
        GameManager.SpawnBoom(transform.position); // spawn boom
        invulnerability = 1f; // grant 1 sec iframe
    }

    private void FixedUpdate()
    {
        Vector2 dir = new(dirX, dirY);
        dir = Vector2.ClampMagnitude(dir, 1f);

        rBody.velocity = (Input.GetButton("Fire1") ? 0.5f : 1f) * speed * dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            int b = (int)Enum.Parse(typeof(Border), collision.gameObject.name);
            borderTouch |= b; // border flagging using bitmask
        }
        else if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (Invulnerable) return; // iframe
            Destroy(collision.gameObject); // destroy enemy bullet
            Die();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            int b = (int)Enum.Parse(typeof(Border), collision.gameObject.name);
            borderTouch ^= b; // border unflagging using bitmask
        }
    }

    public void Die()
    {
        canControl = false; dirX = 0f; dirY = 0f;
        rBody.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        Invoke(nameof(ReallyDie), 1f);
    }

    private void ReallyDie()
    {
        GameManager.Instance().PlayerDie();
        GameManager.SpawnBoom(transform.position); // this also clears bullets
        if (power > 1)
        { // if player had power, spawn 1 power and throw up to let you pick it up
            var pow = GameManager.SpawnItem(transform.position, ItemType.Power);
            pow.GetComponent<Rigidbody2D>().velocity = Vector2.up * 4f;
        }
        GameManager.ShakeCam(2f);
        Destroy(gameObject);
    }

}
