using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Melee Attack")]
public class MeleeAttackState : AIState
{
    public GameObject hitboxObject;
    public string attackAnimationTrigger;

    public override void OnEnter(Enemy enemy)
    {
        enemy.activeAttackHitbox = Instantiate(hitboxObject, enemy.transform);

        enemy.activeAttackHitbox.transform.localPosition = Vector3.zero;
        enemy.activeAttackHitbox.transform.localRotation = Quaternion.identity;
        enemy.activeAttackHitbox.transform.localScale = Vector3.one;
        enemy.activeAttackHitbox.SetActive(false);

        Animator enemyAnim = enemy.GetComponent<Animator>();
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enemyAnim != null)
            enemyAnim.SetTrigger(attackAnimationTrigger);
        
        // Holy crap I hate this solution to this problem
        if (!EnemyAttackManager.instance.RequestAttack(enemy))
            return;

        enemy.transform.rotation = Quaternion.identity;
    }

    public override void OnExit(Enemy enemy)
    {
        Destroy(enemy.activeAttackHitbox);
    }

    public override void OnUpdate(Enemy enemy)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();

        if (player == null)
            return;

        if (hitboxObject == null || enemy.stateTimer < 0.5)
        {
            Vector3 attackDir = player.position - enemy.transform.position;
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            enemy.activeAttackHitbox.transform.position = enemy.transform.position;
            enemy.activeAttackHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (enemyRenderer != null)
            {
                if (player.position.x > enemy.transform.position.x)
                    enemyRenderer.flipX = true;
                else
                    enemyRenderer.flipX = false;
            }

            return;
        }

        enemy.activeAttackHitbox.SetActive(true);
    }

    // Unused
    public override void OnFixedUpdate(Enemy enemy) {}
}
