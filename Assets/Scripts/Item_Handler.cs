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


    private void OnEnable()
    {
        eaten = false;
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.2f;
        rb.velocity = Vector2.down * 0.5f;
        CancelInvoke(nameof(Deactivate));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eaten) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        switch (type)
        {
            case ItemType.Coin:
                GameManager.Instance().AddCoin(1);
                break;
            case ItemType.Power:
                var pc = collision.gameObject.GetComponent<Player_Controller>();
                if (pc.power >= 3) { pc.power = 3; GameManager.Instance().AddCoin(3); }
                else pc.power++;
                break;
            case ItemType.Boom:
                GameManager.Instance().AddBoom(1);
                break;
        }
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse); // "fling up and falls down" animation
        eaten = true;
        Invoke(nameof(Deactivate), 2f);
    }

    private void Deactivate() => gameObject.SetActive(false);

}
