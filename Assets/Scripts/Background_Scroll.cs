using UnityEngine;

public class Background_Scroll : MonoBehaviour
{
    public float speed = 1f;

    private float height;

    private void Start()
    {
        var box = GetComponent<BoxCollider2D>();
        height = box.size.y;
        box.enabled = false;
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.down);
        if (transform.position.y < -height) transform.Translate(height * 2f * Vector2.up);
    }
}
