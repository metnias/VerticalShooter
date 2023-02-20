using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public GameObject bulletPrefabA;
    public GameObject bulletPrefabB;

    public float speed = 4f;
    public int power = 1;
    public float[] bulletDelay;

    private Animator animator;
    private Rigidbody2D rBody;

    private const string ANIMX = "DirX";

    private float dirX = 0f;
    private float dirY = 0f;
    private float bulletCooldown = 0f;

    private int borderTouch = 0;

    [Flags]
    private enum Border
    {
        Top = 1 << 1, Bottom = 1 << 2, Left = 1 << 3, Right = 1 << 4
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
        Fire();
        ReloadBullet();
    }

    private void Move()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        if ((borderTouch & (int)Border.Left) > 0 && dirX < 0f) dirX = 0f;
        else if ((borderTouch & (int)Border.Right) > 0 && dirX > 0f) dirX = 0f;

        dirY = Input.GetAxisRaw("Vertical");
        if ((borderTouch & (int)Border.Bottom) > 0 && dirY < 0f) dirY = 0f;
        else if ((borderTouch & (int)Border.Top) > 0 && dirY > 0f) dirY = 0f;

        animator.SetInteger(ANIMX, dirX == 0 ? 0 : (int)Mathf.Sign(dirX));
    }

    private void Fire()
    {
        if (!Input.GetButton("Fire1")) return;
        if (bulletCooldown < bulletDelay[power - 1]) return;
        bulletCooldown = 0f;

        Power();
    }

    private void Power()
    {
        switch (power)
        {
            case 1:
                {
                    var bullet = Instantiate(bulletPrefabA, transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                }
                break;
            case 2:
                {
                    var bullet = Instantiate(bulletPrefabA, transform.position + Vector3.right * 0.15f, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);

                    bullet = Instantiate(bulletPrefabA, transform.position + Vector3.left * 0.15f, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                }
                break;
            default:
            case 3:
                {
                    var bullet = Instantiate(bulletPrefabA, transform.position + Vector3.right * 0.3f, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);

                    bullet = Instantiate(bulletPrefabA, transform.position + Vector3.left * 0.3f, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);

                    bullet = Instantiate(bulletPrefabB, transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().AddForce(8f * Vector3.up, ForceMode2D.Impulse);
                }
                break;
        }

    }

    private void ReloadBullet()
    {
        bulletCooldown += Time.deltaTime;
    }


    private void FixedUpdate()
    {
        Vector2 dir = new(dirX, dirY);
        dir = Vector2.ClampMagnitude(dir, 1f);

        rBody.velocity = speed * dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            int b = (int)Enum.Parse(typeof(Border), collision.gameObject.name);
            borderTouch |= b;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            int b = (int)Enum.Parse(typeof(Border), collision.gameObject.name);
            borderTouch ^= b;
        }
    }


}
