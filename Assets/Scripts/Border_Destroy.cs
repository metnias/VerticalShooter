using UnityEngine;

public class Border_Destroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            Destroy(gameObject);
    }

}
