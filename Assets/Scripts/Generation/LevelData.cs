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
    
    [Header("Map Parameters")]
    public int mapWidth;
    public int mapHeight;
    [FormerlySerializedAs("MaxMapRooms")] public int maxMapRooms = 1;
    public int maxRoomsPerBranch;
    public float extraHallsChance;
    public float specialRoomsChance;
    public bool CanLoop;
    public bool MapDepthPenalty;
    public bool AttemptSpreadBalancing;
    public Room.ConnectionDirection[] validDirections;
    
    [Header("Map Randomization")]
    [FormerlySerializedAs("RandomizerSeed")] public int GeneratorRandomizerSeed;
    //public int EnemySelectionSeed; Removed because it led to only one type of enemy ever spawning
    
    [Header("Room Spacing")]
    public int roomOffset;
    
    public Room GetStartRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>(GeneratorRandomizerSeed);
        
        foreach (WeightedRoom item in startRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetEndRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>(GeneratorRandomizerSeed);
        
        foreach (WeightedRoom item in endRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetSpecialRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>(GeneratorRandomizerSeed);
        
        foreach (WeightedRoom item in specialRooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }
    
    public Room GetRoom()
    {
        ItemSelector<Room> selector = new ItemSelector<Room>(GeneratorRandomizerSeed);
        
        foreach (WeightedRoom item in rooms)
            selector.AddItem((Room)item.connectable, item.weight);

        return selector.Roll();
    }

    public Hallway GetHallway()
    {
        ItemSelector<Hallway> selector = new ItemSelector<Hallway>(GeneratorRandomizerSeed);
        
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
