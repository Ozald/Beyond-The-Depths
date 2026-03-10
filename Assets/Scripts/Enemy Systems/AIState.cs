using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public abstract void OnEnter(Enemy enemy);
    public abstract void OnUpdate(Enemy enemy);
    public abstract void OnExit(Enemy enemy);
}
