using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Melee Attack")]
public class MeleeAttackState : AIState
{
    public GameObject hitboxObject;
    public string attackAnimationTrigger;

    private const string attackHitboxKey = "ActiveAttackHitbox";

    public override void OnEnter(Enemy enemy)
    {
        enemy.transform.rotation = Quaternion.identity;

        if (!EnemyAttackManager.instance.RequestAttack(enemy))
            return;

        Animator enemyAnim = enemy.GetComponent<Animator>();
        GameObject attackHitbox = Instantiate(hitboxObject, enemy.transform);

        // Store the active attack hitbox in the enemy's context for later use
        enemy.GetContext().SetAttribute(attackHitboxKey, attackHitbox);

        attackHitbox.transform.localPosition = Vector3.zero;
        attackHitbox.transform.localRotation = Quaternion.identity;
        attackHitbox.transform.localScale = Vector3.one;
        attackHitbox.SetActive(false);

        if (enemyAnim != null)
            enemyAnim.SetTrigger(attackAnimationTrigger);
    }

    public override void OnExit(Enemy enemy)
    {
        GameObject attackHitbox = enemy.GetContext().GetAttribute<GameObject>(attackHitboxKey);

        if (attackHitbox is not null)
        {
            enemy.GetContext().DeleteAttribute(attackHitboxKey);
            Destroy(attackHitbox);
        }
    }

    public override void OnUpdate(Enemy enemy)
    {
        Transform player = PlayerManager.instance.transform;
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        GameObject attackHitbox = enemy.GetContext().GetAttribute<GameObject>(attackHitboxKey);

        if (player == null || attackHitbox == null)
            return;

        if (enemy.stateTimer < 0.5)
        {
            Vector3 attackDir = player.position - enemy.transform.position;
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            attackHitbox.transform.position = enemy.transform.position;
            attackHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (enemyRenderer != null)
            {
                if (player.position.x > enemy.transform.position.x)
                    enemyRenderer.flipX = true;
                else
                    enemyRenderer.flipX = false;
            }

            return;
        }

        attackHitbox.SetActive(true);
    }

    // Unused
    public override void OnFixedUpdate(Enemy enemy) {}
}
