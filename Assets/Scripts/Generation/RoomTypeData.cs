using System;
using UnityEngine;

public enum RoomType
{
    StartRoom,
    EndRoom,
    SpecialRoom,
    Room,
    Hallway,
}


[Serializable]
public struct WeightedRoom
{
    public Connectable connectable;
    public int weight;
}

[CreateAssetMenu(fileName = "Room Types", menuName = "ScriptableObjects/RoomTypes", order = 1)]
public class RoomTypeData : ScriptableObject
{
    public WeightedRoom[] startRooms;
    public WeightedRoom[] endRooms;
    public WeightedRoom[] specialRooms;
    public WeightedRoom[] hallways;
    public WeightedRoom[] rooms;
    
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
}
