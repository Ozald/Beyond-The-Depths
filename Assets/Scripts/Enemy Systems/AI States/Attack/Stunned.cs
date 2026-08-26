using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stunned : AIState
{
    public string stunnedAnimationTrigger = "Stunned";

    public override void OnEnter(Enemy enemy)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        Rigidbody2D chud = GameObject.FindGameObjectWithTag("Chud").GetComponent<Rigidbody2D>();

        rb.velocity = Vector2.zero;
        CameraShake.ShakeCamera(2f, 0.2f, false);

        if (chud is not null)
        {
            chud.transform.position = enemy.transform.position;
            chud.GetComponent<SpriteRenderer>().enabled = true;
            chud.GetComponent<Collider2D>().enabled = true;

            Animator animator = enemy.GetComponent<Animator>();
            if (animator is not null)
            {
                animator.SetTrigger(stunnedAnimationTrigger);
            }

           Vector2 attackDir = -enemy.transform.up;
            chud.AddForce(attackDir * 10, ForceMode2D.Impulse);
        }
    }

    public override void OnExit(Enemy enemy)
    {
        Rigidbody2D chud = GameObject.FindGameObjectWithTag("Chud").GetComponent<Rigidbody2D>();

        if (chud is not null)
        {
            chud.transform.position = enemy.transform.position;
            chud.GetComponent<SpriteRenderer>().enabled = false;
            chud.GetComponent<Collider2D>().enabled = false;
        }
    }

    public override void OnFixedUpdate(Enemy enemy)
    {
    }

    public override void OnUpdate(Enemy enemy)
    {
    }
}
