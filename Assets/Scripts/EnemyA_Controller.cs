using UnityEngine;

public class EnemyA_Controller : Enemy_Controller
{
    public string bulletPoolName = "PoolEBulletC";
    private Pool_Manager bulletPool;

    private void Start()
    {
        bulletPool = GameObject.Find(bulletPoolName).GetComponent<Pool_Manager>();
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

        if (bulletPool.TryDequeue(out var bullet))
        {
            bullet.transform.position = transform.position;
            bullet.GetComponent<Rigidbody2D>().AddForce(SPEED * Vector3.down, ForceMode2D.Impulse);
        }
    }

    protected override void Die()
    {
        if (Random.value < 0.4f)
        {
            GameManager.SpawnItem(transform.position, ItemType.Coin);
        }
        base.Die();
    }
}
