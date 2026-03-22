using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/Transition Conditions/Timer")]
public class TimerCondition : Condition
{
    public float timeToTransition = 5f;

    public override bool Check(Enemy enemy)
    {
        if (enemy.stateTimer > timeToTransition)
            return true;

        return false;
    }
}
