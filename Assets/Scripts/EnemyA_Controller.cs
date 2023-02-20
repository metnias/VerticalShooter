using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA_Controller : Enemy_Controller
{

    protected override void Update()
    {
        base.Update();
        rBody.velocity = Vector3.down * 4f;
    }
}
