using UnityEngine;

public enum RoomType
{
    StartRoom,
    EndRoom,
    SpecialRoom,
    Room,
    Hallway,
}

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/RoomData", order = 1)]
public class RoomDataScript : ScriptableObject
{
    [Header("Room Type")]
    public RoomType roomType;
}
