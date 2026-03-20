using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/Transition Conditions/Distance To Player")]
public class DistanceToPlayer : Condition
{
    public float minDistance = 0f;
    public float maxDistance = 20f;

    public override bool Check(Enemy enemy)
    {
        // This is not the best way to do this, but to avoid merge conflicts I am going to keep it like this. Sue me.
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (Vector3.Distance(player.position, enemy.transform.position) > minDistance && Vector3.Distance(player.position, enemy.transform.position) < maxDistance)
        {
            return true;
        }

        return false;
    }
}
