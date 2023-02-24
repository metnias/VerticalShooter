using UnityEngine;

/// <summary>
/// Circular camera shake
/// (because random vector camera shake would look too frantic for this shooting game)
/// </summary>
public class Camera_Shake : MonoBehaviour
{
    private float shake = 0f;

    private Vector3 anchorPos;

    public void SetShake(float value) => shake = Mathf.Max(shake, value);

    private void Start()
    {
        anchorPos = transform.position; // get anchor pos
    }

    private const float SPD = 40f; // how fast the camera spins

    private void Update()
    {
        if (shake > 0f)
        {
            shake -= Time.deltaTime * 5f;
            Vector3 dir = new Vector2(Mathf.Cos(Time.timeSinceLevelLoad * SPD),
                Mathf.Sin(Time.timeSinceLevelLoad * SPD));
            transform.position = anchorPos + dir * Mathf.Clamp(shake * 0.15f, 0.1f, 0.6f);
        }
        else
        {
            shake = 0f;
            transform.position = anchorPos;
        }

    }
}
