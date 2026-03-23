using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TransitionMode : byte
{
    FirstValid,         // The first valid transition found will be picked
    Random,             // A random valid transition would be picked disregarding its probability weights
    WeightedRandom      // A random valid transition would be picked based on its probability weights
}

[Serializable]
public struct TransitionDestination
{
    public int weight;
    public AIState toState;
    public Condition condition;
}

[Serializable]
public struct EnemyStateTransition
{
    public TransitionMode transitionMode;
    public AIState fromState;
    public List<TransitionDestination> destinations;
}

[CreateAssetMenu(menuName = "Enemy AI/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public int HP;
    public float invincibilityCooldown;

    [Header("State Machine")]
    public AIState initalState;
    public List<EnemyStateTransition> transitions;
}
