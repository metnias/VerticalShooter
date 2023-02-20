using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA_Controller : Enemy_Controller
{

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
        rBody.velocity = vel * 4f;
    }
}
