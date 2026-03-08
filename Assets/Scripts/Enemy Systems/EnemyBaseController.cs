using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct EnemyStateTransition {
    public AIState fromState;
    public AIState toState;
    public Condition condition;
}

public class EnemyBaseController : MonoBehaviour
{
    [Header("Setup")]

    [SerializeField] private AIState initalState;
    [SerializeField] private List<EnemyStateTransition> transitions;

    public float stateTimer { get; private set; }
    [SerializeField] private AIState currentState;


    void Start()
    {
        /*
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D coll = GetComponent<BoxCollider2D>(); //for the base enemy's sprite
        coll.isTrigger = true;
        coll.size =  spriteRenderer.bounds.size;
        */

        stateTimer = 0;
        currentState = initalState;
        currentState.OnEnter();
    }
    
    void Update()
    {
        stateTimer += Time.deltaTime;
        currentState.OnUpdate();

        foreach (EnemyStateTransition transition in transitions)
        {
            // This runs if the enemy finds a state to transition to and the condition to transition is met
            if (transition.fromState == currentState && transition.condition.Check(this))
            {
                currentState.OnExit();
                currentState = transition.toState;
                currentState.OnEnter();
                stateTimer = 0;

                break;
            }
        }
    }
}
