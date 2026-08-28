using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

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
    
    [CanBeNull] private Connectable left;
    [CanBeNull] private Connectable up;
    [CanBeNull] private Connectable right;
    [CanBeNull] private Connectable down;

    [Header("Doors")]
    [CanBeNull] public Door leftDoor;
    [CanBeNull] public Door rightDoor;
    [CanBeNull] public Door upDoor;
    [CanBeNull] public Door downDoor;

    [Header("Room Settings")]
    public RoomType roomType;
    //public SpecificRoomType specificType;
    public GameObject wall;
    public BoundingBox boundingBox;
    public bool SpawnsEnemies;
    public int MinEnemySpawnAttempts;
    public int MaxEnemySpawnAttempts;
    public LayerMask wallLayer;

    [Header("Spawning Particles")]
    public ParticleSystem spawnParticles;
    
    [Header("Debug")]
    public LevelData levelData;
    public TilemapCollider2D wallCollider;
    public bool hasBeenExplored = false;
    public List<Enemy> spawnedEnemies = new List<Enemy>();

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

    void Start()
    {
        StartCoroutine(initRoom());

        if (MaxEnemySpawnAttempts == 0 || !SpawnsEnemies)
            OpenDoors();

        if (roomType == RoomType.EndRoom)
            OpenDoors();

        // This is scuffed
        boundingBox.OnEntered += PlayerEntered;

        wallCollider = wall.GetComponent<TilemapCollider2D>();
    }

    private IEnumerator initRoom()
    {
        yield return null;

        TilemapCollider2D[] tilemaps = GetComponentsInChildren<TilemapCollider2D>();
        foreach (TilemapCollider2D tm in tilemaps)
        {
            CompositeCollider2D compColl = tm.GetComponent<CompositeCollider2D>();
            tm.usedByComposite = false;
            tm.ProcessTilemapChanges();
            tm.usedByComposite = true;
        }

        Physics2D.SyncTransforms();
    }

    void Update()
    {
        if(EnemyManager.instance.currentRoom == this 
            && EnemyManager.instance.AllEnemiesDead() && EnemyManager.instance.allEnemiesSpawned && !hasBeenExplored)
            OpenDoors();

        // Debug.Log("All enemies dead: " + EnemyManager.instance.AllEnemiesDead() + 
                  // " | Current Room: " + EnemyManager.instance.currentRoom.name + " | All Enemies Spawned: " + EnemyManager.instance.allEnemiesSpawned +
                  // " | Has been explored: " + hasBeenExplored);
    }

    void PlayerEntered()
    {
        if(!hasBeenExplored)
            EnemyAttackManager.instance.Enemies.Clear();

        if (hasBeenExplored)
        {
            OpenDoors();
            return;
        }
        
        if (SpawnsEnemies && MinEnemySpawnAttempts > 0)
        {
            EnemyManager.instance.allEnemiesSpawned = false;
            StartCoroutine(SpawnEnemies());
        }

        if (!EnemyManager.instance.currentRoom.SpawnsEnemies && !hasBeenExplored)
            EnemyManager.instance.allEnemiesSpawned = true;

        if (!hasBeenExplored && MinEnemySpawnAttempts > 0)
        {
            if (leftDoor is not null)
                leftDoor.enabled = false;
        
            if (rightDoor is not null)
                rightDoor.enabled = false;
        
            if (downDoor is not null)
                downDoor.enabled = false;
        
            if (upDoor is not null)
                upDoor.enabled = false;
        }
    }

    private IEnumerator<YieldInstruction> SpawnEnemies()
    {
        EnemyManager.instance.currentRoom = this;

        if (MaxEnemySpawnAttempts == 0)
            yield break;
        
        yield return new WaitForSeconds(1f);

        int spawnAttempts = Random.Range(MinEnemySpawnAttempts, MaxEnemySpawnAttempts + 1);

        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 spawnPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            // Temporary?
            spawnPos.x += Random.Range(-10, 30);
            spawnPos.y += Random.Range(-10, 30);

            if (!boundingBox.collider.OverlapPoint(spawnPos))
            {
                //Debug.Log("Out of bounds spawning");
                i--;
                continue;
            }

            Collider2D hitObstacle = Physics2D.OverlapCircle(spawnPos, 1f, wallLayer);

            if (hitObstacle is not null)
            {
                //Debug.Log("Skipping due to overlapping wall");
                i--;
                continue;
            }

            if (boundingBox.collider.OverlapPoint(spawnPos) && !wallCollider.OverlapPoint(spawnPos))
            {
                ParticleSystem particle = Instantiate(spawnParticles, new Vector3(spawnPos.x, spawnPos.y, -1),
                    Quaternion.identity);
                particle.Play();

                yield return new WaitForSeconds(1f);

                Enemy enemy = levelData.GetEnemy();
                EnemyManager.instance.enemyCount++;
                Instantiate(enemy.gameObject, spawnPos, Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
                Destroy(particle.gameObject);
                
                EnemyAttackManager.instance.Enemies.Add(enemy);
                //Debug.Log("Enemy spawned. Enemies: " + EnemyManager.instance.enemyCount);
                spawnedEnemies.Add(enemy);

                yield return new WaitForSeconds(0.2f);
            }
        }
        
        EnemyManager.instance.allEnemiesSpawned = true;
    }

    private void OpenDoors()
    {
        if (hasBeenExplored)
            return;
        
        hasBeenExplored = true;
        
        if (leftDoor is not null)
            leftDoor.enabled = true;
        
        if (rightDoor is not null)
            rightDoor.enabled = true;
        
        if (downDoor is not null)
            downDoor.enabled = true;
        
        if (upDoor is not null)
            upDoor.enabled = true;
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
