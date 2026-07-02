using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public enum RoomType
{
    Room,
    StartRoom,
    EndRoom,
    SpecialRoom
}

[Serializable]
public struct WeightedRoom
{
    public Connectable connectable;
    public int weight;
}

[Serializable]
public struct WeightedEnemy
{
    public Enemy enemy;
    public int weight;
}

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Room Types")]
    public WeightedRoom[] rooms;
    public WeightedRoom[] startRooms;
    public WeightedRoom[] endRooms;
    public WeightedRoom[] specialRooms;
    public WeightedRoom[] hallways; // Effectively deprecated
    
    [Header("Enemy Spawns")]
    public WeightedEnemy[] enemies;
    
    [Header("Global Map Parameters")]
    public int mapWidth;
    public int mapHeight;
    [FormerlySerializedAs("MaxMapRooms")] public int maxMapRooms = 10;
    [Range(0, 1)] public float specialRoomsChance;
    [Range(0, 1)] public float extraHallsChance;
    public bool canLoop;
    public Room.ConnectionDirection[] validDirections = new Room.ConnectionDirection[]
    {
        Room.ConnectionDirection.Up,
        Room.ConnectionDirection.Down,
        Room.ConnectionDirection.Right,
        Room.ConnectionDirection.Left
    };
    
    public TileGraph.Algorithm algorithm;
    
    [Header("Standard Generation Parameters")]
    public int maxRoomsPerBranch;
    public bool mapDepthPenalty;
    public bool attemptSpreadBalancing;
    
    [Header("Alternative Generation Parameters (Breadth First/Depth First)")] 
    [Range(0, 1)] public double generationChance = 0.5;
    [Range(0, 1)] public double generationChanceDecay = 0.05; // This is additive, not multiplicative
    [Range(0, 4)] [FormerlySerializedAs("MaximumBranchAttempts")] public int maximumBranchAttempts = 4;
    
    [Header("Map Randomization")]
    [FormerlySerializedAs("RandomizerSeed")] public int generatorRandomizerSeed;
    //public int EnemySelectionSeed; Removed because it led to only one type of enemy ever spawning
    
    [Header("Room Spacing")]
    public int roomOffset;
    
    public Room GetStartRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>();
        
        foreach (WeightedRoom item in startRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetEndRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>();
        
        foreach (WeightedRoom item in endRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetSpecialRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>();
        
        foreach (WeightedRoom item in specialRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>();
        
        foreach (WeightedRoom item in rooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }

    public Hallway GetHallway()
    {
        ItemSelector<Hallway> selector = new ItemSelector<Hallway>();
        
        foreach (WeightedRoom item in hallways)
            selector.AddItem((Hallway)item.connectable, item.weight);

        return selector.Roll();
    }

    public Enemy GetEnemy()
    {
        ItemSelector<Enemy> selector = new ItemSelector<Enemy>();

        foreach(WeightedEnemy e in enemies)
            selector.AddItem(e.enemy, e.weight);
        
        return selector.Roll();
    }
}
