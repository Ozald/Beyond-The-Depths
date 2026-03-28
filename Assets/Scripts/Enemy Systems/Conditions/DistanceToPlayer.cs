using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComparisonOperator
{
    LessThan,
    GreaterThan
}

[CreateAssetMenu(menuName = "Enemy AI/Transition Conditions/Distance To Player")]
public class DistanceToPlayer : Condition
{
    public float distanceThreshold = 20f;
    public ComparisonOperator comparisonOperator = ComparisonOperator.LessThan;

    public override bool Check(Enemy enemy)
    {
        // This is not the best way to do this, but to avoid merge conflicts I am going to keep it like this. Sue me.
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (comparisonOperator == ComparisonOperator.LessThan && Vector3.Distance(player.position, enemy.transform.position) < distanceThreshold)
            return true;
        if (comparisonOperator == ComparisonOperator.GreaterThan && Vector3.Distance(player.position, enemy.transform.position) > distanceThreshold)
            return true;

        return false;
    }
}
