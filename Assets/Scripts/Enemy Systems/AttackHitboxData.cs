using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackHitboxData : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public float maxLifetime;
    public int damage;
    public float knockback;
    public bool destroyOnHit = false;
    public bool crit;
    public ParticleSystem critEffect; // Only needed for player weapons

    public LayerMask wallLayer;
    public LayerMask layerToHit;
    public bool usesAddForce = false;

    private float currLifetime;
    private Rigidbody2D rb;

    private HashSet<EnemyHP> enemiesHit = new HashSet<EnemyHP>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currLifetime = 0f;
    }

    void Update()
    {
        if (rb != null && !usesAddForce)
            rb.velocity = direction * speed;

        currLifetime += Time.deltaTime;
        if (currLifetime < maxLifetime)
            return;

        if (transform.parent != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        EnemyHP enemy = collider.gameObject.GetComponent<EnemyHP>();
        // StatsManager player = collider.gameObject.GetComponent<StatsManager>();

        if (enemy != null && (layerToHit & (1 << collider.gameObject.layer)) != 0)
        {
            if (enemiesHit.Contains(enemy) || enemy.timeSinceLastHit < enemy.enemyData.invincibilityCooldown)
                return;

            if (crit && critEffect is not null)
            {
                ParticleSystem effect = Instantiate(critEffect, transform.position, transform.rotation);
                effect.Play();
            }

            enemy.TakeDamage(damage, knockback);
            enemiesHit.Add(enemy);

            if (destroyOnHit)
                Destroy(gameObject);
        }

        if (wallLayer == (wallLayer | (1 << collider.gameObject.layer)))
        {
            Destroy(gameObject);
        }
    }
}
