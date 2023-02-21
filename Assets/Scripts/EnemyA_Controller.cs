using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA_Controller : Enemy_Controller
{
    public GameObject bulletPrefab;

    private void Start()
    {
        fireDelay = 0.5f;
    }

    protected override void Update()
    {
        base.Update();
        Vector3 vel = Vector3.down;
        if (player != null)
        {
            if (player.transform.position.y < transform.position.y)
            {
                vel.x = player.transform.position.x - transform.position.x;
                vel = Vector3.ClampMagnitude(vel, 1f);
            }
        }
        rBody.velocity = vel * 2.5f;
    }

    protected override void Fire()
    {
        base.Fire();

        if (player == null) return;

        const float SPEED = 7f;

        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(SPEED * Vector3.down, ForceMode2D.Impulse);
    }

}
