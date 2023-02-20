using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB_Controller : Enemy_Controller
{

    protected override void Update()
    {
        base.Update();
        rBody.velocity = Vector3.down * 3f;
    }
}
