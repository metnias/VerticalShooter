using UnityEngine;

public class UI_Bump : MonoBehaviour
{
    public RectTransform panel;

    private Vector2 origPos;
    private Vector2 origPanelPos;
    private Rigidbody2D rBody;

    public bool lockRight = true;
    public bool lockUp = true;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        origPos = transform.position;
        origPanelPos = (Vector2)panel.position - origPos;
    }

    private void Update()
    {
        panel.position = origPanelPos + (Vector2)transform.position;
    }

    private void FixedUpdate()
    {
        if (origPos.x == transform.position.x && origPos.y == transform.position.y) return;

        Vector3 clamp = transform.position;
        if (lockRight) clamp.x = Mathf.Min(clamp.x, origPos.x);
        else clamp.x = Mathf.Max(clamp.x, origPos.x);
        if (lockUp) clamp.y = Mathf.Min(clamp.y, origPos.y);
        else clamp.y = Mathf.Max(clamp.y, origPos.y);

        Vector2 dir = origPos - (Vector2)transform.position;
        rBody.velocity = Vector2.ClampMagnitude(dir * 8f, 8f);
    }
}
