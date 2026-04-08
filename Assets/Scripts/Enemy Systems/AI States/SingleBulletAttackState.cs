using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Single Bullet Attack")]
public class SingleBulletAttackState : AIState
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

        enemy.StartCoroutine(SpawnProjectile(enemy));
    }

    public override void OnExit(Enemy enemy) {}

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

    IEnumerator SpawnProjectile(Enemy enemy)
    {
        yield return new WaitForSeconds(0.5f);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 attackDir = (player.position - enemy.transform.position).normalized;
        AttackHitboxData projectile = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        projectile.speed = projectileSpeed;
        projectile.direction = new Vector2(attackDir.x, attackDir.y);
        projectile.maxLifetime = projectileLifetime;
        projectile.damage = 1;
    }
}
