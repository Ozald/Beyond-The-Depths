using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/8-Way Bullet Attack")]
public class EightWayAttackState : AIState
{
    public AttackHitboxData projectilePrefab;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 5f;
    public string attackAnimationTrigger;

    public override void OnEnter(Enemy enemy)
    {
        Animator enemyAnim = enemy.GetComponent<Animator>();
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();

        if (enemyAnim != null)
            enemyAnim.SetTrigger(attackAnimationTrigger);

        enemy.transform.rotation = Quaternion.identity;

        if (projectilePrefab == null)
            return;

        enemy.StartCoroutine(SpawnProjectiles(enemy));
    }

    public override void OnExit(Enemy enemy) { }

    public override void OnUpdate(Enemy enemy)
    {
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enemyRenderer != null && player != null)
        {
            if (player.position.x > enemy.transform.position.x)
                enemyRenderer.flipX = true;
            else
                enemyRenderer.flipX = false;
        }
    }

    // Unused
    public override void OnFixedUpdate(Enemy enemy) { }

    IEnumerator SpawnProjectiles(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector2[] attackDirections = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(1, -1),
            new Vector2(0, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
        };

        foreach (Vector2 attack in attackDirections)
        {
            AttackHitboxData projectile = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
            projectile.speed = projectileSpeed;
            projectile.direction = new Vector2(attack.x, attack.y).normalized;
            projectile.maxLifetime = projectileLifetime;
            projectile.damage = 1;
        }
    }
}
