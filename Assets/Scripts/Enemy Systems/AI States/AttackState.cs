using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/MeleeAttack")]
public class AttackState : AIState
{
    public GameObject hitboxObject;

    public override void OnEnter(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " entering attack state.");

        enemy.activeAttackHitbox = Instantiate(hitboxObject, null);

        enemy.activeAttackHitbox.transform.localPosition = Vector3.zero;
        enemy.activeAttackHitbox.transform.localRotation = Quaternion.identity;
        enemy.activeAttackHitbox.transform.localScale = Vector3.one;
        enemy.activeAttackHitbox.SetActive(false);
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " exiting attack state.");

        Destroy(enemy.activeAttackHitbox);
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (hitboxObject == null || enemy.stateTimer < 0.5)
            return;

        enemy.activeAttackHitbox.SetActive(true);
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 attackDir = player.position - enemy.transform.position;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        enemy.activeAttackHitbox.transform.position = enemy.transform.position;
        enemy.activeAttackHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
