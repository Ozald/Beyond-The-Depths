using JetBrains.Annotations;
using UnityEngine;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has entered the room.");
        }
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
