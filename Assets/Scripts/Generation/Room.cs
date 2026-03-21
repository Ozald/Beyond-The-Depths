using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : Connectable
{
    // void Awake()
    // {
    //     Debug.Log(name + "(Awake): "  + GetComponent<Collider2D>().enabled);
    // }
    //
    // void Start()
    // { 
    //     Debug.Log(name + "(Start): "  + GetComponent<Collider2D>().enabled);
    // }

    // For the love of god and everything that is good I cannot find what is disabling this object
    // Demonic code requires demonic answers
    //void OnDisable()
    //{
    //    Debug.LogError($"{name} was disabled\n{System.Environment.StackTrace}", this);
    //    GetComponent<Room>().enabled = true;
    //    GetComponent<Collider2D>().enabled = true;
    //}

    public enum ConnectionDirection
    {
        Left,
        Up,
        Right,
        Down
    }

    private int maxConnections;
    public LevelData levelData;

    [CanBeNull] private Connectable left;
    [CanBeNull] private Connectable up;
    [CanBeNull] private Connectable right;
    [CanBeNull] private Connectable down;

    [CanBeNull] public Door leftDoor;
    [CanBeNull] public Door rightDoor;
    [CanBeNull] public Door upDoor;
    [CanBeNull] public Door downDoor;
    [FormerlySerializedAs("spawnpoints")] public Spawnpoint[] enemySpawnpoints;

    public RoomType roomType;
    public bool hasBeenExplored = false;

    public Connectable? Left
    {
        get { return left; }
        set { left = value; }
    }

    public Connectable? Up
    {
        get { return up; }
        set { up = value; }
    }

    public Connectable? Right
    {
        get { return right; }
        set { right = value; }
    }

    public Connectable? Down
    {
        get { return down; }
        set { down = value; }
    }

    public int MaxConnections
    {
        get { return maxConnections; }
        set { maxConnections = value; }
    }

    public Room()
    {
        Connections = new[] { Left, Up, Right, Down };
        maxConnections = 4;
    }

    public Room(int maxConnections)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = maxConnections;
    }

    public Room(bool isOrigin)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = 4;

        if (isOrigin)
            roomType = RoomType.StartRoom;
    }

    public Room(int maxConnections, bool isOrigin)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = maxConnections;

        if (isOrigin)
            roomType = RoomType.StartRoom;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenExplored && enemySpawnpoints.Length > 0)
            SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        HashSet<Spawnpoint> enemySpawns = new HashSet<Spawnpoint>();
        int enemiesToSpawn = Random.Range(1, enemySpawnpoints.Length + 1);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Spawnpoint spawnpoint = enemySpawnpoints[Random.Range(0, enemySpawnpoints.Length)];
            enemySpawns.Add(spawnpoint);
        }
        
        foreach (Spawnpoint point in enemySpawns)
        {
            if(point is null)
                continue;
            
            if(point.hasSpawned)
                continue;
            
            Enemy enemy = levelData.GetEnemy();
            Instantiate(enemy.gameObject, point.transform.position, point.transform.rotation);
            point.hasSpawned = true;
        }
    }

    public override string ToString()
    {
        if (roomType == RoomType.StartRoom)
            return "O";

        if (roomType == RoomType.EndRoom)
            return "E";

        if (roomType == RoomType.SpecialRoom)
            return "S";

        return "R";
    }
}
