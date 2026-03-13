using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Chase")]
public class ChaseState : AIState
{
    public float moveSpeed;

    public override void OnEnter(Enemy enemy)
    {
        AIPath enemyAI = enemy.GetComponent<AIPath>();

        if (enemyAI != null)
            enemyAI.isStopped = false;
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " exiting chase state.");

        AIPath enemyAI = enemy.GetComponent<AIPath>();

        if (enemyAI != null)
            enemyAI.isStopped = true;
    }

    public override void OnUpdate(Enemy enemy)
    {
        AIPath enemyAI = enemy.GetComponent<AIPath>();

        if (enemyAI != null)
        {
            enemyAI.maxSpeed = moveSpeed;

            // This is not the best way to do this, but to avoid merge conflicts I am going to keep it like this. Sue me.
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            enemyAI.destination = player.position;
        }
    }
}
