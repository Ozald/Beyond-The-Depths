using JetBrains.Annotations;
using UnityEngine;

public class Room : Connectable
{
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

    [CanBeNull] public Door leftDoor;
    [CanBeNull] public Door rightDoor;
    [CanBeNull] public Door upDoor;
    [CanBeNull] public Door downDoor;
    
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

    public override string ToString()
    {
        if (roomType == RoomType.StartRoom)
        {
            return "O";
        }

        if (roomType == RoomType.EndRoom)
        {
            return "E";
        }

        if (roomType == RoomType.SpecialRoom)
        {
            return "S";
        }

        return "R";
    }
}
