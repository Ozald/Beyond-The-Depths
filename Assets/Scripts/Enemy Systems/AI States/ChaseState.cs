using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Chase")]
public class ChaseState : AIState
{
    public float moveSpeed;
    public string chaseAnimationTrigger;

    public override void OnEnter(Enemy enemy)
    {
        AIPath enemyAI = enemy.GetComponent<AIPath>();
        Animator enemyAnim = enemy.GetComponent<Animator>();

        if (enemyAI != null)
            enemyAI.isStopped = false;

        if (enemyAnim != null)
            enemyAnim.SetTrigger(chaseAnimationTrigger);
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " exiting chase state.");
        
    }

    public override void OnUpdate(Enemy enemy) {}

    public override void OnFixedUpdate(Enemy enemy) 
    {
        Seeker enemyAI = enemy.GetComponent<Seeker>();
        Rigidbody2D enemyRB = enemy.GetComponent<Rigidbody2D>();

        if (enemyAI != null && enemyRB != null)
        {
            CalculatePath(enemy);

            if (enemy.currentPath == null)
                return;

            if (enemy.currentWaypoint >= enemy.currentPath.vectorPath.Count)
            {
                enemy.reachedEndOfPath = true;
                return;
            }
            else
            {
                enemy.reachedEndOfPath = false;
            }

            // How the enemy traverses the path (Rigidbody for a floaty feel)

            Vector2 moveDir = ((Vector2)enemy.currentPath.vectorPath[enemy.currentWaypoint] - enemyRB.position).normalized;

            enemyRB.AddForce(moveDir * moveSpeed);

            // To make the enemy face the direction of where it is moving

            Vector2 dir = (Vector2)enemy.currentPath.vectorPath[enemy.currentWaypoint] - enemyRB.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.Euler(0, 0, angle - 90), 10f * Time.fixedDeltaTime);

            // To continue to the next waypoint in the path

            float distance = Vector2.Distance(enemyRB.position, enemy.currentPath.vectorPath[enemy.currentWaypoint]);
            if (distance < 2f)
            {
                enemy.currentWaypoint++;
            }
        }
    }

    /***********************************************************************************/

    void CalculatePath(Enemy enemy)
    {
        Seeker enemyAI = enemy.GetComponent<Seeker>();

        // This is not the best way to do this, but to avoid merge conflicts I am going to keep it like this. Sue me.
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        enemyAI.StartPath(enemy.transform.position, player.position, (Path p) =>
        {
            if (!p.error)
            {
                enemy.currentPath = p;
                int newWaypoint = 0;

                for (int i = 0; i < p.vectorPath.Count; i++)
                {
                    float distance = Vector2.Distance(enemy.transform.position, p.vectorPath[i]);
                    if (distance >= 1f)
                    {
                        newWaypoint = i;
                        break;
                    }
                }

                enemy.currentWaypoint = newWaypoint;
            }
        });
    }
}
