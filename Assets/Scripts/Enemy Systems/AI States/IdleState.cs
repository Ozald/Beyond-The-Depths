using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/Idle")]
public class IdleState : AIState
{
    // This class rn does literally nothing until further notice
    public override void OnEnter(Enemy enemy)
    {
        //Debug.Log(enemy.gameObject.name + " entering idle state.");
    }

    public override void OnExit(Enemy enemy)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnUpdate(Enemy enemy)
    {
        //Debug.Log(enemy.gameObject.name + " exiting idle state.");
    }

    public override void OnFixedUpdate(Enemy enemy)
    {
        //throw new System.NotImplementedException();
    }
}
