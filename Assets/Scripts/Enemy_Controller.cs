using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    protected Rigidbody2D rBody;

    public Sprite hitSprite;
    private Sprite ownSprite;
    protected SpriteRenderer spr;

    public int health = 1;
    private float hitShow = 0f;

    protected GameObject player;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        ownSprite = spr.sprite;
        player = GameObject.FindGameObjectWithTag("Player");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            OnHit(collision.gameObject.GetComponent<Bullet_Handler>().power);
            

        }
    }

    protected void OnHit(int power)
    {
        health -= power;
        if (health > 0)
        {
            hitShow = 0.5f;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
