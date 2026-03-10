using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AIState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " entering chase state.");
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " exiting chase state.");
    }

    public override void OnUpdate(Enemy enemy)
    {
        Seeker enemyAI = enemy.GetComponent<Seeker>();

        if (enemyAI != null)
        {
            //TODO: go to the player's location
        }
    }
}
