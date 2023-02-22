using UnityEngine;

public class Border_Destroy : MonoBehaviour
{
    public bool inPool = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            if (inPool) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }

}
