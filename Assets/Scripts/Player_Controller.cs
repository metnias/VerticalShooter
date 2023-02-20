using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public float speed = 4f;

    private Animator animator;
    private Rigidbody2D rBody;

    private const string ANIMX = "DirX";

    private float dirX = 0f;
    private float dirY = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");
        animator.SetInteger(ANIMX, dirX == 0 ? 0 : (int)Mathf.Sign(dirX));
    }

    private void FixedUpdate()
    {
        Vector2 dir = new Vector2(dirX, dirY);
        dir = Vector2.ClampMagnitude(dir, 1f);

        rBody.velocity = speed * dir;
    }
}
