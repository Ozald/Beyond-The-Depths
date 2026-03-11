using System;
using System.Collections.Generic;
using UnityEngine;

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

/**************************************************************************************/

public class Enemy : MonoBehaviour
{
    [Header("Setup")]

    [SerializeField] private AIState initalState;
    [SerializeField] private List<EnemyStateTransition> transitions;

    public float stateTimer { get; private set; }
    [SerializeField] private AIState currentState;

    /**************************************************************************************/

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
        currentState.OnEnter(this);
    }
    
    void Update()
    {
        stateTimer += Time.deltaTime;
        currentState.OnUpdate(this);

        TransitionHandler();
    }

    /**************************************************************************************/

    private void TransitionHandler()
    {
        if (stateTimer < 1f)
            return;

        // Find every valid destination in a transition, store in a list, and then pick one of them
        foreach (EnemyStateTransition transition in transitions)
        {
            if (transition.fromState != currentState)
                continue;

            List<TransitionDestination> validDestinations = new List<TransitionDestination>();

            foreach (TransitionDestination destination in transition.destinations)
            {
                if (destination.condition.Check(this))
                    validDestinations.Add(destination);
            }

            if (validDestinations.Count > 0)
                PickValidDestination(validDestinations, transition.transitionMode);
        }
    }

    private void PickValidDestination(List<TransitionDestination> destinations, TransitionMode transitionMode)
    {
        currentState.OnExit(this);

        // Pick a valid destination based on the transition mode
        AIState newState = null;

        switch (transitionMode)
        {
            case TransitionMode.FirstValid:
                newState = destinations[0].toState;
                break;

            case TransitionMode.Random:
                newState = destinations[UnityEngine.Random.Range(0, destinations.Count)].toState;
                break;

            case TransitionMode.WeightedRandom:

                AIState randomDestination = null;

                int total = 0;
                destinations.ForEach(destination => total += destination.weight);

                int random = UnityEngine.Random.Range(1, total + 1);
                int cursor = 0;
                for (int i = 0; i < destinations.Count; i++)
                {
                    cursor += destinations[i].weight;
                    if (cursor >= random)
                        randomDestination = destinations[i].toState;
                }

                newState = randomDestination;
                break;
        }

        if (newState != currentState)
        {
            currentState.OnExit(this);
            currentState = newState;
            currentState.OnEnter(this);
            stateTimer = 0;
        }
    }
}
