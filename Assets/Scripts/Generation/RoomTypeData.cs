using UnityEngine;

public enum RoomType
{
    StartRoom,
    EndRoom,
    SpecialRoom,
    Room,
    Hallway,
}

[CreateAssetMenu(fileName = "Room Types", menuName = "ScriptableObjects/RoomTypes", order = 1)]
public class RoomTypeData : ScriptableObject
{
    public Room[] startRooms;
    public Room[] endRooms;
    public Room[] specialRooms;
    public Hallway[] hallways;
    public Room[] rooms;
    
    public Room GetStartRoom()
    {
        return startRooms[Random.Range(0, startRooms.Length)];
    }

    public Room GetEndRoom()
    {
        return endRooms[Random.Range(0, endRooms.Length)];
    }

    public Room GetSpecialRoom()
    {
        return specialRooms[Random.Range(0, specialRooms.Length)];
    }
    
    public Room GetRoom()
    {
        return rooms[Random.Range(0, rooms.Length)];
    }

    public Hallway GetHallway()
    {
        return hallways[Random.Range(0, hallways.Length)];
    }
}
