using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PathRequest : IEquatable<PathRequest>
{
    public Enemy Enemy;
    public Action Callback;

    public PathRequest(Enemy enemy, Action callback)
    {
        this.Enemy = enemy;
        this.Callback = callback;
    }

    public bool Equals(PathRequest other)
    {
        if (Enemy == other.Enemy)
            return true;

        return false;
    }
}

public class PathAIManager : MonoBehaviour
{
    public static PathAIManager instance;

    public int maxEnemiesToProcessPerFrame = 5;
    public float timeBetweenProcessBatches = 0.5f;
    private Buffer<PathRequest> pathRequestBuffer;

    void Awake()
    {
        if (instance == null)
            instance = this;    
    }

    void Start()
    {
        pathRequestBuffer = new Buffer<PathRequest>();
        InvokeRepeating("ProcessPaths", 0f, timeBetweenProcessBatches);
    }

    void ProcessPaths()
    {
        HashSet<PathRequest> pathsToProcess = pathRequestBuffer.RemoveBudgetNoDuplicates(budget: maxEnemiesToProcessPerFrame, reAdd: false);

        foreach (PathRequest request in pathsToProcess)
        {
            request.Callback();
        }
    }

    public void RequestPathUpdate(Enemy enemy, Action callback)
    {
        PathRequest newRequest = new PathRequest(enemy, callback);
        if (pathRequestBuffer.Contains(newRequest))
            return;

        pathRequestBuffer.Add(newRequest);
    }
}
