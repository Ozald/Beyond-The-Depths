using System;
using UnityEngine;

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
    public WeightedRoom[] startRooms;
    public WeightedRoom[] specialRooms;
    public WeightedRoom[] hallways;
    public WeightedRoom[] rooms;
    public WeightedRoom[] endRooms;
    public WeightedEnemy[] enemies;
    
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
