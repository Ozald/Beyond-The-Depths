using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/MeleeAttack")]
public class AttackState : AIState
{
    // This class rn does literally nothing until further notice
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " entering attack state.");
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log(enemy.gameObject.name + " exiting attack state.");
    }

    public override void OnUpdate(Enemy enemy)
    {
        // nothin lol
    }
}
