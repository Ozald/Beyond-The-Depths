using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/States/DEBUG")]
public class DEBUG_State : AIState
{
    public override void OnEnter(Enemy enemy)
    {
        base.OnEnter(enemy);
    }

    public override void OnUpdate(Enemy enemy)
    {
        base.OnUpdate(enemy);
    }

    public override void OnExit(Enemy enemy) 
    { 
        base.OnExit(enemy);
    }
}
