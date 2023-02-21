using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Mathf;

public class EnemyC_Controller : Enemy_Controller
{
    public GameObject bulletPrefab;

    private void Start()
    {
        fireDelay = 1.5f;
    }

    protected override void Update()
    {
        base.Update();
        Vector3 vel = Vector3.down;
        if (player != null)
        {
            if (player.transform.position.y < transform.position.y)
            {
                vel.x = Mathf.Clamp(player.transform.position.x - transform.position.x, -1f, 1f);
                vel = Vector3.ClampMagnitude(vel, 1f);
            }
        }
        rBody.velocity = vel * 1f;
    }

    protected override void Fire()
    {
        base.Fire();

        if (player == null) return;

        float angle = Atan2(player.transform.position.y - transform.position.y,
            player.transform.position.x - transform.position.x) * Rad2Deg;
        if (angle > 0f) angle = -90f;
        angle = Clamp(angle, -150f, -30f);
        float angleL = angle + 15f, angleR = angle - 15f;
        if (angleL > 180f) angleL -= 360f;
        if (angleR < 180f) angleR += 360f;

        const float SPEED = 3f;

        var dir = new Vector2(Cos(angle * Deg2Rad), Sin(angle * Deg2Rad));
        var bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0f, -0.2f), Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);

        dir = new Vector2(Cos(angleL * Deg2Rad), Sin(angleL * Deg2Rad));
        bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0.2f, -0.2f), Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);

        dir = new Vector2(Cos(angleR * Deg2Rad), Sin(angleR * Deg2Rad));
        bullet = Instantiate(bulletPrefab, transform.position + new Vector3(-0.2f, -0.2f), Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);

    }

}
