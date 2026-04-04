using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxData : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public float maxLifetime;
    public int damage;
    public int knockback;

    public string tagToHit;

    private float currLifetime;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currLifetime = 0f;
    }

    void Update()
    {
        if (rb != null)
            rb.velocity = direction * speed;

        currLifetime += Time.deltaTime;
        if (currLifetime > maxLifetime)
            Destroy(gameObject);
    }
}
