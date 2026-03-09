using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public virtual void OnEnter(Enemy enemy)
    {
        Debug.Log("Entered state");
    }

    public virtual void OnUpdate(Enemy enemy)
    {
        Debug.Log("Updating state");
    }

    public virtual void OnExit(Enemy enemy)
    {
        Debug.Log("Exiting state");
    }
}
