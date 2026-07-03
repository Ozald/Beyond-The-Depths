using System;
using UnityEngine;
using UnityEngine.Serialization;

// General room types
public enum RoomType
{
    Room,
    StartRoom,
    EndRoom,
    SpecialRoom
}

// Very specific room type
public enum SpecificRoomType
{
    Start,
    End,
    Combat,
    Shop,
    Chest,
    Boss, // Not implemented yet
    Healing, // Not implemented yet
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
    
    [Tooltip("The pool of rooms")]
    public WeightedRoom[] rooms;
    
    [Tooltip("The pool of start rooms")]
    public WeightedRoom[] startRooms;
    
    [Tooltip("The pool of end rooms")]
    public WeightedRoom[] endRooms;
    
    [Tooltip("The pool of special rooms")]
    public WeightedRoom[] specialRooms;
    
    [Tooltip("The pool of hallways")]
    public WeightedRoom[] hallways; // Effectively deprecated
    
    [Header("Enemy Spawns")]
    
    [Tooltip("The pool of enemies")]
    public WeightedEnemy[] enemies;
    
    [Header("Global Map Parameters")]
    
    [Tooltip("The maximum width of the map")]
    public int mapWidth;
    
    [Tooltip("The maximum height of the map")]
    public int mapHeight;
    
    [Tooltip("The maximum rooms on the map")]
    [FormerlySerializedAs("MaxMapRooms")] public int maxMapRooms = 10;
    
    [Tooltip("The probability of a special room spawning instead of a normal room")]
    [Range(0, 1)] public float specialRoomsChance;
    
    [Tooltip("If the map can form cycles")]
    public bool canLoop;
    
    [Tooltip("The probability of an extra doorway being created to form cycles in the map")]
    [Range(0, 1)] public float extraHallsChance;
    
    [Tooltip("The directions the algorithm can generate along")]
    public Room.ConnectionDirection[] validDirections = new Room.ConnectionDirection[]
    {
        Room.ConnectionDirection.Up,
        Room.ConnectionDirection.Down,
        Room.ConnectionDirection.Right,
        Room.ConnectionDirection.Left
    };
    
    [Header("Algorithm")]
    
    [Tooltip("The algorithm being used to generate the map")]
    public TileGraph.Algorithm algorithm;
    
    [Header("Standard Generation Parameters (Does not apply to breadth first or depth first generation")]
    
    [Tooltip("Maximum rooms per branch of the standard algorithm")]
    public int maxRoomsPerBranch;
    
    [Tooltip("If there is a penalty for deeper map generation")]
    public bool mapDepthPenalty;
    
    [Tooltip("If the algorithm should attempt to spread the rooms out across each direction")]
    public bool attemptSpreadBalancing;
    
    [Header("Alternative Generation Parameters (Breadth First/Depth First)")]
    
    [Tooltip("The number of rooms to generate before the odds of generating more begin to decrease")]
    public int GenerationChanceBuffer;
    
    [Tooltip("The probability of a room generating")]
    [Range(0, 1)] public double generationChance = 0.5;
    
    [Tooltip("The amount of decay in room generation chance per room generated")]
    [Range(0, 1)] public double generationChanceDecay = 0.05; // This is additive, not multiplicative
    
    [Tooltip("The number of attempts that can be made to branch out in generation")]
    [Range(0, 4)] public int maximumBranchAttempts = 4;

    [Header("Map Randomization")]
    
    [Tooltip("If a randomized seed should be provided to generation")]
    public bool provideRandomizedSeed = true;
    
    [Tooltip("The seed to use for generation if a random seed is not provided")]
    public int generatorRandomizerSeed;
    // public int EnemySelectionSeed; Removed because it led to only one type of enemy ever spawning
    
    [Header("Room Spacing")]
    
    [Tooltip("The amount of offset between the center of each room")]
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
