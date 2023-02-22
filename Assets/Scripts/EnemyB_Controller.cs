using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB_Controller : Enemy_Controller
{
    public string bulletPoolName = "PoolEBulletA";
    private Pool_Manager bulletPool;

    private void Start()
    {
        bulletPool = GameObject.Find(bulletPoolName).GetComponent<Pool_Manager>();
        fireDelay = 0.8f;
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

        if (bulletPool.TryDequeue(out var bullet))
        {
            bullet.transform.position = transform.position + new Vector3(0f, -0.2f);
            bullet.GetComponent<Rigidbody2D>().AddForce(dir * SPEED, ForceMode2D.Impulse);
        }
    }

    protected override void Die()
    {
        float rnd = Random.value;
        if (rnd < 0.2f)
        {
            GameManager.SpawnItem(transform.position, ItemType.Boom);
        }
        else if (rnd < 0.8f)
        {
            GameManager.SpawnItem(transform.position, ItemType.Coin);
        }
        base.Die();
    }
}
