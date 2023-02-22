using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType : int
{
    Coin = 0,
    Power = 1,
    Boom = 2
}

public class Item_Handler : MonoBehaviour
{
    public ItemType type;

    private bool eaten = false;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eaten) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        switch (type)
        {
            case ItemType.Coin:
                GameManager.Instance().AddCoin(1);
                GetComponent<CircleCollider2D>().enabled = false;
                break;
            case ItemType.Power:
                var pc = collision.gameObject.GetComponent<Player_Controller>();
                pc.power = Mathf.Clamp(pc.power + 1, 1, 3);
                GetComponent<BoxCollider2D>().enabled = false;
                break;
            case ItemType.Boom:
                GameManager.Instance().AddBoom(1);
                GetComponent<BoxCollider2D>().enabled = false;
                break;
        }
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        eaten = true;
        Invoke(nameof(Deactivate), 2f);
    }

    private void Deactivate() => gameObject.SetActive(false);
    
}
