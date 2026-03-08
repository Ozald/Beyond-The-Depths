using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public virtual void OnEnter()
    {
        Debug.Log("Entered state");
    }

    public virtual void OnUpdate()
    {
        Debug.Log("Updating state");
    }

    public virtual void OnExit()
    {
        Debug.Log("Exiting state");
    }
}
