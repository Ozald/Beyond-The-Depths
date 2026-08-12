using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/**************************************************************************************/

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    private EnemyContext context;   // This is used to store any data that the states or transitions need to share with each other, such as a reference to the current attack hitbox.

    public float stateTimer { get; private set; }
    [SerializeField] private AIState currentState;

    // Pathfinding storage, use the state machine system to modify these values
    [NonSerialized] public Path currentPath;
    [NonSerialized] public int currentWaypoint;
    [NonSerialized] public bool reachedEndOfPath = true;
    public bool debugMode = false;

    /**************************************************************************************/

    void Start()
    {
        context = new EnemyContext();
        currentPath = null;
        currentWaypoint = 0;

        stateTimer = 0;
        currentState = enemyData.initialState;
        currentState.OnEnter(this);
    }

    void FixedUpdate()
    {
        currentState.OnFixedUpdate(this);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        currentState.OnUpdate(this);

        if (debugMode)
            Debug.LogWarning($"Current State: {currentState.GetType().Name}");

        TransitionHandler();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /**************************************************************************************/

    public EnemyContext GetData()
    {
        return context;
    }

    /**************************************************************************************/

    private void TransitionHandler()
    {
        // Find every valid destination in a transition, store in a list, and then pick one of them
        foreach (StateNode transition in enemyData.states)
        {
            if (transition.state != currentState)
                continue;

            List<Transition> validDestinations = new List<Transition>();
            
            foreach (Transition destination in transition.transitions)
            {
                bool validTransition = true;
                foreach (Condition condition in destination.conditions)
                {
                    if (!condition.Check(this))
                        validTransition = false;
                }

                if (validTransition)
                    validDestinations.Add(destination);
            }

            if (validDestinations.Count > 0)
                PickValidDestination(validDestinations, transition.transitionMode);
        }
    }

    private void PickValidDestination(List<Transition> destinations, TransitionMode transitionMode)
    {
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

        currentState.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
        stateTimer = 0;
    }
}
