using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB_Controller : Enemy_Controller
{

    public GameObject bulletPrefab;

    private void Start()
    {
        fireDelay = 1f;
    }

    protected override void Update()
    {
        base.Update();
        Vector3 vel = Vector3.down;
        if (player != null)
        {
            if (player.transform.position.y < transform.position.y)
            {
                vel.x = Mathf.Clamp(player.transform.position.x - transform.position.x, -2f, 2f);
                vel = Vector3.ClampMagnitude(vel, 1f);
            }
        }
        rBody.velocity = vel * 1.5f;
    }

    protected override void Fire()
    {
        base.Fire();

        if (player == null) return;

        Vector2 dir = player.transform.position - transform.position;
        if (dir.y > 0f) dir = Vector2.down;
        else dir = dir.normalized;

        const float SPEED = 4f;

        var bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0f, -0.2f), Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);


    }
}
