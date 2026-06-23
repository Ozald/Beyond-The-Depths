using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class AIState
{
    public abstract void OnEnter(Enemy enemy);
    public abstract void OnUpdate(Enemy enemy);
    public abstract void OnFixedUpdate(Enemy enemy);
    public abstract void OnExit(Enemy enemy);
    
}
