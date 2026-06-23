using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

public class TileGraph : MonoBehaviour
{
    public enum Half
    {
        Left,
        Right,
        Up,
        Down
    }

    private int width;
    private int height;
    private Connectable[,] grid;
    private int maxRoomsPerBranch;
    private readonly Random random;
    private readonly Dictionary<Room, Vector2> rooms;
    private readonly Dictionary<Hallway, Vector2> halls;
    private Vector2 startPos;
    private Room? Start;
    public float extraHallsChance;
    public float specialRoomsChance;
    public int offset;
    public int MaximumRooms;

    public bool CanLoop;
    public bool EnableDepthPenalty;
    public bool AttemptBalancing;

    // For the experimental alternative generation algorithm
    public bool UseAlternateGeneration;
    public double GenerationChance;
    public int MaximumBranchAttempts;
    public double GenerationChanceReduction;
    
    public Room.ConnectionDirection[] validDirections;

    public LevelData roomTypes;

    public float ExtraHallsChance { get => extraHallsChance; set => extraHallsChance = value; }

    public int Width
    {
        get { return width; }
        set
        {
            if (value < 3)
            {
                throw new ArgumentException("Width must be >= 3.");
            }

            width = value;
        }
    }

    public int Height
    {
        get { return height; }
        set
        {
            if (value < 3)
            {
                throw new ArgumentException("Height must be >= 3");
            }

            height = value;
        }
    }

    public int RoomCount
    {
        get { return rooms.Count; }
    }

    public int HallCount
    {
        get { return halls.Count; }
    }

    public Vector2 StartPosition
    {
        get { return startPos; }
        private set { startPos = value; }
    }

    public int MaxRoomsPerBranch
    {
        get { return maxRoomsPerBranch; }
        set { maxRoomsPerBranch = value; }
    }

    public Connectable[,] Grid
    {
        get { return grid; }
        private set { grid = value; }
    }

    public Room? StartRoom
    {
        get { return Start; }
        private set { Start = value; }
    }

    public TileGraph(int width, int height, int maxRoomsPerBranch, int randomSeed)
    {
        this.width = width;
        this.height = height;
        this.maxRoomsPerBranch = maxRoomsPerBranch;
        rooms = new();
        halls = new();
        startPos = new Vector2(-1, -1);

        grid = new Connectable[width, height];

        random = new Random(randomSeed);
    }

    // Depth-first generation
    public void GenerateMap(Vector2 startPosition)
    {
        Debug.Log("Starting generation");
        Debug.Log($"Start position: {startPosition}");
        
        if (!InBounds(startPosition))
            throw new ArgumentException("Start position is not in the map.");
        
        if(UseAlternateGeneration)
        {
            GenerateAlternative(startPosition, GenerationChance);
            
            if (CanLoop)
                AddHalls(extraHallsChance);
        }
        else
        {
            // Create the starting room and start the recursive generation
            Room startRoom = Instantiate(roomTypes.GetStartRoom().gameObject, 
                new Vector3(startPosition.x * offset, startPosition.y * offset, 0), Quaternion.identity).GetComponent<Room>();
            StartPosition = startPosition;
            Start = startRoom;
            rooms.Add(startRoom, startPosition);
            grid[(int)startPosition.x, (int)startPosition.y] = startRoom;

            startRoom.upDoor.enabled = true;
            startRoom.downDoor.enabled = true;
            startRoom.leftDoor.enabled = true;
            startRoom.rightDoor.enabled = true;
        
            GenerateFrom(startRoom, startPosition, 0, (int)(MaxRoomsPerBranch * 0.75), out startRoom);

            if (CanLoop)
                AddHalls(extraHallsChance);
        }

        SetEndRoom();
        CleanDoors();
        
        Debug.Log($"Generated {rooms.Count} rooms.");
        Debug.Log($"Generated {halls.Count} halls.");
    }

    // An alternative, simpler procedural generation algorithm
    public void GenerateAlternative(Vector2 startPosition, double generationChance)
    {
        Room startRoom = Instantiate(roomTypes.GetStartRoom().gameObject, 
            new Vector3(startPosition.x * offset, startPosition.y * offset, 0), Quaternion.identity).GetComponent<Room>();
        StartPosition = startPosition;
        Start = startRoom;
        rooms.Add(startRoom, startPosition);
        grid[(int)startPosition.x, (int)startPosition.y] = startRoom;

        startRoom.levelData = roomTypes;
        
        startRoom.leftDoor.enabled = true;
        startRoom.rightDoor.enabled = true;
        startRoom.upDoor.enabled = true;
        startRoom.downDoor.enabled = true;

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(startRoom);

        List<Vector2> directions = new();
        foreach (Room.ConnectionDirection dir in validDirections)
        {
            if (dir == Room.ConnectionDirection.Up)
                directions.Add(new Vector2(0, 2));
            
            if(dir == Room.ConnectionDirection.Down)
                directions.Add(new Vector2(0, -2));
            
            if(dir == Room.ConnectionDirection.Left)
                directions.Add(new Vector2(-2, 0));
            
            if(dir == Room.ConnectionDirection.Right)
                directions.Add(new Vector2(2, 0));
        }

        while (roomQueue.Count > 0 && rooms.Count < MaximumRooms)
        {
            Room origin = roomQueue.Dequeue();
            origin.levelData = roomTypes;
            Vector2 position = rooms[origin];
            
            // Set up the doors
            if(origin.upDoor is not null)
                origin.upDoor.parentRoom = origin;
            
            if(origin.downDoor is not null)
                origin.downDoor.parentRoom = origin;
            
            if(origin.leftDoor is not null)
                origin.leftDoor.parentRoom = origin;
            
            if(origin.rightDoor is not null)
                origin.rightDoor.parentRoom = origin;

            for (int i = 0; i < MaximumBranchAttempts; i++)
            {
                if (rooms.Count >= MaximumRooms)
                    break;
                
                if (random.NextDouble() < generationChance)
                {
                    Room next = roomTypes.GetRoom().GetComponent<Room>();
                    Vector2 direction = directions[random.Next(0, directions.Count)];
                    
                    if (PlaceAt(ref next, position + direction))
                    {
                        next.leftDoor.enabled = true;
                        next.rightDoor.enabled = true;
                        next.upDoor.enabled = true;
                        next.downDoor.enabled = true;

                        next.leftDoor.parentRoom = next;
                        next.rightDoor.parentRoom = next;
                        next.upDoor.parentRoom = next;
                        next.downDoor.parentRoom = next;

                        rooms.Add(next, position + direction);
                        
                        if (direction.Equals(directions[0]) && origin.Left is null)
                        {
                            origin.Left = next;

                            if (next.rightDoor is not null)
                            {
                                next.rightDoor.connectedDoor = origin.leftDoor;
                                origin.leftDoor.connectedDoor = next.rightDoor;
                            }
                        }
                        else if (direction.Equals(directions[1]) && origin.Right is null)
                        {
                            origin.Right = next;

                            if (next.leftDoor is not null)
                            {
                                next.leftDoor.connectedDoor = origin.rightDoor;
                                origin.rightDoor.connectedDoor = next.leftDoor;
                            }
                        }
                        else if (direction.Equals(directions[2]) && origin.Up is null)
                        {
                            origin.Up = next;

                            if (next.upDoor is not null)
                            {
                                next.upDoor.connectedDoor = origin.downDoor;
                                origin.downDoor.connectedDoor = next.upDoor;
                            }
                        }
                        else if (direction.Equals(directions[3]) && origin.Down is null)
                        {
                            origin.Down = next;

                            if (next.downDoor is not null)
                            {
                                next.downDoor.connectedDoor = origin.upDoor;
                                origin.upDoor.connectedDoor = next.downDoor;
                            }
                        }

                        generationChance -= GenerationChanceReduction;
                        roomQueue.Enqueue(next);
                    }
                }
            }
        }
    }

    // Recursive helper for standard depth-first generation
    [CanBeNull]
    private Room GenerateFrom(Room start, Vector2 startVector, int roomsGenerated, int penaltySafety, out Room room)
    {
        start.levelData = roomTypes;
        if (roomsGenerated > maxRoomsPerBranch || rooms.Count >= MaximumRooms)
        {
            room = null;
            return null;
        }

        if (!InBounds(startVector))
        {
            room = null;
            return null;
        }

        // Prevent generating rooms on top of each other.
        // This should also stop hallways from overwriting things.
        // This was really only here because the origin room kept
        // getting overwritten.
        if (!IsSpotEmpty(startVector) && start != Start)
        {
            room = null;
            return null;
        }

        if (start is null)
        {
            Debug.Log("Start room is null.");
            room = null;
            return null;
        }
        
        if (!rooms.ContainsKey(start))
        {
            PlaceAt(ref start, startVector);
            rooms.Add(start, startVector);
        }
        
        // start.upDoor = Instantiate(doorPrefab.gameObject, start.transform.position + new Vector3(0, -1, 0),
        //     Quaternion.identity).GetComponent<Door>();
        
        if(start.upDoor is not null)
            start.upDoor.parentRoom = start;
        
        // start.downDoor = Instantiate(doorPrefab.gameObject, start.transform.position + new Vector3(0, 1, 0),
        //     Quaternion.identity).GetComponent<Door>();
        
        if(start.downDoor is not null)
            start.downDoor.parentRoom = start;
        
        // start.leftDoor = Instantiate(doorPrefab.gameObject, start.transform.position + new Vector3(-1, 0, 0),
        //     Quaternion.identity).GetComponent<Door>();
        
        if(start.leftDoor is not null)
            start.leftDoor.parentRoom = start;
        
        // start.rightDoor = Instantiate(doorPrefab.gameObject, start.transform.position + new Vector3(1, 0, 0),
        //     Quaternion.identity).GetComponent<Door>();
        if(start.rightDoor is not null)
            start.rightDoor.parentRoom = start;

        Debug.Log("Layer: " + roomsGenerated);
        
        // Randomize the order that we generate the directions in.
        // Honestly quite dumb the way we had to get this.
        PriorityQueue<Room.ConnectionDirection> connections = new();

        if (AttemptBalancing)
        {
            foreach (Room.ConnectionDirection direction in Enum.GetValues(typeof(Room.ConnectionDirection)))
            {
                // If no direction is valid, this will probably explode the generation
                if (!validDirections.Contains(direction))
                    continue;

                // Attempt to bring some balance to the distribution of the rooms
                Half leastPopulated = LeastPopulatedHalf();
                float multiplier = 1 - roomsGenerated * 0.03f * (float)random.NextDouble();

                if (leastPopulated == Half.Up && direction == Room.ConnectionDirection.Up)
                    connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
                else if (leastPopulated == Half.Down && direction == Room.ConnectionDirection.Down)
                    connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
                else if (leastPopulated == Half.Left && direction == Room.ConnectionDirection.Left)
                    connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
                else if (leastPopulated == Half.Right && direction == Room.ConnectionDirection.Right)
                    connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
                else
                    connections.Enqueue(direction, random.Next(4));
            }
        }
        else
        {
            foreach (Room.ConnectionDirection direction in Enum.GetValues(typeof(Room.ConnectionDirection)))
                connections.Enqueue(direction, 1);
        }

        // Add some connections, maybe not all
        int connectionCount = 0;
        int usedConnections = (int)Math.Ceiling(start.MaxConnections *
                                                (1d - (double)roomsGenerated / maxRoomsPerBranch)) + random.Next(-1, 1);

        // Heuristic to decrease the amount of depth we can get from the recursion
        if (roomsGenerated > penaltySafety && EnableDepthPenalty)
        {
            float depthPenalty = CalculateDepthPenalty(roomsGenerated);
            usedConnections = (int)Math.Floor(usedConnections * depthPenalty);
        }

        // Generate the branching halls and rooms
        while (connectionCount < usedConnections && connections.Count > 0)
        {
            Room.ConnectionDirection direction = connections.Dequeue();

            switch (direction)
            {
                case Room.ConnectionDirection.Left:
                    //random.Next(roomsGenerated < maxRoomsPerBranch / 4 ? 2 : 1, 5)
                    Room tempLeft = roomTypes.GetRoom().GetComponent<Room>();
                    
                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom(tempLeft, startVector + new Vector2(-2, 0), roomsGenerated + 1,
                            penaltySafety, out tempLeft) is not null)
                    {
                        Console.WriteLine("Generating left branch");
                        start.Left = tempLeft;
                        PlaceHallAt(startVector + new Vector2(-1, 0), start, tempLeft);
                        connectionCount++;
                        
                        // I hate this so much
                        tempLeft.Right = start;

                        // Linking doors
                        if (start.leftDoor is not null)
                        {
                            start.leftDoor.connectedDoor = tempLeft.rightDoor;
                            tempLeft.rightDoor.connectedDoor = start.leftDoor;
                        }
                    }
                    else
                    {
                        start.Left = null;
                    }

                    break;
                case Room.ConnectionDirection.Right:
                    Room tempRight = roomTypes.GetRoom().GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom(tempRight, startVector + new Vector2(2, 0), roomsGenerated + 1,
                            penaltySafety, out tempRight) is not null)
                    {
                        Console.WriteLine("Generating left branch");
                        start.Right = tempRight;
                        PlaceHallAt(startVector + new Vector2(1, 0), start, tempRight);
                        connectionCount++;

                        // I hate this so much
                        tempRight.Left = start;

                        // Linking doors
                        if (start.rightDoor is not null)
                        {
                            start.rightDoor.connectedDoor = tempRight.leftDoor;
                            tempRight.leftDoor.connectedDoor = start.rightDoor;
                        }
                    }
                    else
                    {
                        start.Right = null;
                    }

                    break;
                case Room.ConnectionDirection.Up:
                    Room tempUp = roomTypes.GetRoom().GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom(tempUp, startVector + new Vector2(0, -2), roomsGenerated + 1,
                            penaltySafety, out tempUp) is not null)
                    {
                        Console.WriteLine("Generating left branch");
                        start.Up = tempUp;
                        PlaceHallAt(startVector + new Vector2(0, -1), start, tempUp);
                        connectionCount++;

                        // I hate this so much
                        tempUp.Down = start;

                        // Linking doors
                        if (start.upDoor is not null)
                        {
                            start.upDoor.connectedDoor = tempUp.downDoor;
                            tempUp.downDoor.connectedDoor = start.upDoor;
                        }
                    }
                    else
                    {
                        start.Up = null;
                    }

                    break;
                case Room.ConnectionDirection.Down:
                    Room tempDown = roomTypes.GetRoom().GetComponent<Room>();
                    
                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom(tempDown, startVector + new Vector2(0, 2), roomsGenerated + 1,
                            penaltySafety, out tempDown) is not null)
                    {
                        Console.WriteLine("Generating left branch");
                        start.Up = tempDown;
                        PlaceHallAt(startVector + new Vector2(0, 1), start, tempDown);
                        connectionCount++;

                        // I hate this so much
                        tempDown.Up = start;

                        // Linking doors
                        if (start.downDoor is not null)
                        {
                            start.downDoor.connectedDoor = tempDown.upDoor;
                            tempDown.upDoor.connectedDoor = start.downDoor;
                        }
                    }
                    else
                    {
                        start.Down = null;
                    }

                    break;
            }
        }
        
        Debug.Log("Layer: " + roomsGenerated + " connections");
        Debug.Log("Left: " + start.Left);
        Debug.Log("Right: " + start.Right);
        Debug.Log("Up: " + start.Up);
        Debug.Log("Down: " + start.Down);

        room = start;
        return start;
    }

    // Places the end room
    private void SetEndRoom()
    {
        Room farthest = UseAlternateGeneration ? GetFarthestRoom() : GetFarthestDeadEnd();

        if (farthest is not null)
        {
            Room end = Instantiate(roomTypes.GetEndRoom().gameObject, farthest.transform.position, Quaternion.identity).GetComponent<Room>();

            // Not having this almost screwed us over 3 hours before demo
            if(end.leftDoor is not null)
                end.leftDoor.parentRoom = end;
            
            if(end.rightDoor is not null)
                end.rightDoor.parentRoom = end;
            
            if (end.upDoor is not null)
                end.upDoor.parentRoom = end;
            
            if (end.downDoor is not null)
                end.downDoor.parentRoom = end;
            
            if (farthest.leftDoor.connectedDoor is not null)
            {
                farthest.leftDoor.connectedDoor.connectedDoor = end.leftDoor;
                end.leftDoor.connectedDoor = farthest.leftDoor.connectedDoor;
            }

            if (farthest.rightDoor.connectedDoor is not null)
            {
                farthest.rightDoor.connectedDoor.connectedDoor = end.rightDoor;
                end.rightDoor.connectedDoor = farthest.rightDoor.connectedDoor;
            }

            if (farthest.upDoor.connectedDoor is not null)
            {
                farthest.upDoor.connectedDoor.connectedDoor = end.upDoor;
                end.upDoor.connectedDoor = farthest.upDoor.connectedDoor;
            }

            if (farthest.downDoor.connectedDoor is not null)
            {
                farthest.downDoor.connectedDoor.connectedDoor = end.downDoor;
                end.downDoor.connectedDoor = farthest.downDoor.connectedDoor;
            }
            
            Destroy(farthest.gameObject);
        }
    }

    // Adds extra halls to the map
    private void AddHalls(float chance)
    {
        List<Room> roomList = rooms.Keys.ToList();

        for (int i = roomList.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                Room originRoom = roomList[i];
                Room end = roomList[j];

                if (originRoom == end)
                    continue;

                if (originRoom.roomType == RoomType.EndRoom || end.roomType == RoomType.EndRoom)
                    continue;

                if (random.NextDouble() < chance)
                {
                    if (Math.Abs(XDist(originRoom, end)) == 2 && YDist(originRoom, end) == 0)
                    {
                        if (XDist(originRoom, end) == -2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(1, 0), originRoom, end))
                            {
                                Debug.Log("Added extra right hall");
                                if (originRoom.rightDoor is not null)
                                {
                                    originRoom.rightDoor.connectedDoor = end.leftDoor;
                                    end.leftDoor.connectedDoor = originRoom.rightDoor;
                                }
                            }
                        }
                        else if (XDist(originRoom, end) == 2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(-1, 0), originRoom, end))
                            {
                                Debug.Log("Added extra left hall");
                                if (originRoom.leftDoor is not null)
                                {
                                    originRoom.leftDoor.connectedDoor = end.rightDoor;
                                    end.rightDoor.connectedDoor = originRoom.leftDoor;
                                }
                            }
                        }
                    }
                    else if (Math.Abs(YDist(originRoom, end)) == 2 && XDist(originRoom, end) == 0)
                    {
                        if (YDist(originRoom, end) == 2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(0, -1), originRoom, end))
                            {
                                Debug.Log("Added extra up hall");
                                // OH MY GOD, THIS TOOK MULTIPLE DAYS TO SOLVE
                                // originRoom.upDoor = end.downDoor;
                                // to
                                // originRoom.upDoor.connectedDoor = end.downDoor;
                                if (originRoom.upDoor is not null)
                                {
                                    originRoom.upDoor.connectedDoor = end.downDoor;
                                    end.downDoor.connectedDoor = originRoom.upDoor;
                                }
                            }
                        }
                        else if (YDist(originRoom, end) == -2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(0, 1), originRoom, end))
                            {
                                Debug.Log("Added extra down hall");
                                // OH MY GOD, THIS TOOK MULTIPLE DAYS TO SOLVE
                                // originRoom.downDoor = end.upDoor;
                                // to
                                // originRoom.downDoor.connectedDoor = end.upDoor;
                                if (originRoom.downDoor is not null)
                                {
                                    originRoom.downDoor.connectedDoor = end.upDoor;
                                    end.upDoor.connectedDoor = originRoom.downDoor;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Literally needed to get rid of doors
    // without connections because for some reason
    // it didn't work in the recursive method
    public void CleanDoors()
    {
        List<Door> doors = new List<Door>(FindObjectsByType<Door>(FindObjectsSortMode.None));

        foreach (Door door in doors)
        {
            if (door.connectedDoor is null)
                Destroy(door.gameObject);

            if (door.connectedDoor is not null && door.connectedDoor.parentRoom is not null)
            {
                if (door.connectedDoor.parentRoom == door.parentRoom)
                {
                    Destroy(door.connectedDoor.gameObject);
                    Destroy(door.gameObject);
                }
            }
        }
    }

    // Places a hallway
    public bool PlaceHallAt(Vector2 pos, Connectable? origin, Connectable? end)
    {
        Hallway hall = Instantiate(roomTypes.GetHallway().gameObject, 
            new Vector3(pos.x * offset, pos.y * offset, 0), Quaternion.identity).GetComponent<Hallway>();

        hall.Origin = origin;
        hall.End = end;
        
        if (PlaceAt(ref hall, pos))
        {
            halls.Add(hall, pos);
            return true;
        }

        Destroy(hall.gameObject);
        return false;
    }


    // Places an item at a grid position.
    // Holy crap does this solution suck
    public bool PlaceAt(ref Room room, Vector2 pos)
    {
        if (!IsSpotEmpty(pos))
            return false;
        
        if(room.roomType == RoomType.Room && random.NextDouble() < specialRoomsChance)
            room = Instantiate(roomTypes.GetSpecialRoom().gameObject, new Vector3(pos.x * offset, pos.y * offset, 0), Quaternion.identity).GetComponent<Room>();
        else
            room = Instantiate(room.gameObject, new Vector3(pos.x * offset, pos.y * offset, 0), Quaternion.identity).GetComponent<Room>();
        
        grid[(int)pos.x, (int)pos.y] = room;

        return true;
    }
    
    // Places an item at a grid position
    // Holy crap does this solution suck
    public bool PlaceAt(ref Hallway hall, Vector2 pos)
    {
        if (!IsSpotEmpty(pos))
            return false;
        
        hall = Instantiate(hall.gameObject, new Vector3(pos.x * offset, pos.y * offset, 0), Quaternion.identity).GetComponent<Hallway>();
        grid[(int)pos.x, (int)pos.y] = hall;

        return true;
        
    }

    // Gets the feature at a position
    public Connectable? GetAt(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y];
    }

    // Counts the rooms in the top half of the map
    public int CountTopHalf()
    {
        int bound = height / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].y < bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the bottom half of the map
    public int CountBottomHalf()
    {
        int bound = height / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].y > bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the left half of the map
    public int CountLeftHalf()
    {
        int bound = width / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].x < bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the right half of the map
    public int CountRightHalf()
    {
        int bound = width / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].x > bound)
            {
                count++;
            }
        }

        return count;
    }

    // Finds the least populated half of the grid
    public Half LeastPopulatedHalf()
    {
        int leftOrRight = Math.Min(CountRightHalf(), CountLeftHalf());
        int topOrBottom = Math.Min(CountBottomHalf(), CountTopHalf());

        int min = Math.Min(leftOrRight, topOrBottom);

        if (min == leftOrRight)
        {
            if (leftOrRight == CountLeftHalf())
            {
                return Half.Left;
            }

            return Half.Right;
        }

        if (topOrBottom == CountTopHalf())
        {
            return Half.Up;
        }

        return Half.Down;
    }

    // Calculates a penalty to slow increasing of the recursion depth
    private float CalculateDepthPenalty(int roomsGenerated)
    {
        float percentDepth = (float)roomsGenerated / maxRoomsPerBranch;
        float penaltyMultiplier = 1f - percentDepth;

        return penaltyMultiplier;
    }

    // x-coordinate distance between two rooms
    private int XDist(Room room1, Room room2)
    {
        Vector2 room1Pos = rooms[room1];
        Vector2 room2Pos = rooms[room2];

        return (int)(room1Pos.x - room2Pos.x);
    }

    // y-coordinate distance between two rooms.
    private int YDist(Room room1, Room room2)
    {
        Vector2 room1Pos = rooms[room1];
        Vector2 room2Pos = rooms[room2];

        return (int)(room1Pos.y - room2Pos.y);
    }

    // Determines if a position on the grid is empty.
    public bool IsSpotEmpty(Vector2 pos)
    {
        if (!InBounds(pos)) return false;

        return grid[(int)pos.x, (int)pos.y] is null;
    }

    // Determines if a position is within the grid
    private bool InBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x < width && pos.y < height;
    }

    // Determines if a room has only one neighbor
    public bool IsDeadEnd(Room room)
    {
        List<Vector2> directions = new();
        directions.Add(new Vector2(1, 0));
        directions.Add(new Vector2(-1, 0));
        directions.Add(new Vector2(0, -1));
        directions.Add(new Vector2(0, 1));
        
        List<Connectable> list = new();

        if (rooms.TryGetValue(room, out Vector2 pos))
        {
            foreach (Vector2 dir in directions)
            {
                Vector2 neighborPos = pos + dir;

                if (!InBounds(neighborPos))
                    continue;

                if (GetAt(neighborPos) is null)
                    continue;

                list.Add(GetAt(neighborPos));
            }
        }

        return list.Count == 1;
    }

    // Finds the farthest dead end room from the starting point
    public Room? GetFarthestDeadEnd()
    {
        Room farthest = (Room)GetAt(StartPosition);
        double farthestDistance = 0;

        foreach (Room room in rooms.Keys)
        {
            if (IsDeadEnd(room) && room.roomType != RoomType.SpecialRoom && room.roomType != RoomType.StartRoom)
            {
                if (Vector2.Distance(StartPosition, rooms[room]) > farthestDistance)
                {
                    farthest = room;
                    farthestDistance = Vector2.Distance(StartPosition, rooms[room]);
                }
            }
        }
        
        return farthest;
    }

    // Finds the farthest room from the starting point
    public Room? GetFarthestRoom()
    {
        Room farthest = (Room)GetAt(StartPosition);
        double farthestDistance = 0;

        foreach (Room room in rooms.Keys)
        {
            if (room.roomType != RoomType.SpecialRoom && room.roomType != RoomType.StartRoom)
            {
                if (Vector2.Distance(StartPosition, rooms[room]) > farthestDistance)
                {
                    farthest = room;
                    farthestDistance = Vector2.Distance(StartPosition, rooms[room]);
                }
            }
        }
        
        return farthest;
    }
    
    public override string ToString()
    {
        string s = string.Empty;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] is null)
                {
                    s += "- ";
                    continue;
                }

                s += grid[i, j] + " ";
            }

            s += '\n';
        }

        return s;
    }
}
