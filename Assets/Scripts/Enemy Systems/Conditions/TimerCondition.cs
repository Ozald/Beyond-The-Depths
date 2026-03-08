using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/Transition Conditions/Timer")]
public class TimerCondition : Condition
{
    public float timeToTransition = 5f;

    public override bool Check(EnemyBaseController enemy)
    {
        if (enemy.stateTimer > timeToTransition)
        {
            Debug.Log("Switching states...");
            return true;
        }

        return false;
    }
}
