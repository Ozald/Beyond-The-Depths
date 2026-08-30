using Cinemachine;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    public LevelData roomTypes;

    public TileGraph map;
    
    void Start()
    {
        GenerateFloorLayout();
        GenerateNavMesh();
    }

    void GenerateFloorLayout()
    {
        List<Connectable> roomCollection = new List<Connectable>(FindObjectsByType<Connectable>(FindObjectsSortMode.None));
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        foreach (Connectable room in roomCollection)
            Destroy(room.gameObject);

        if (map is null)
        {
            int seed = roomTypes.provideRandomizedSeed ? Random.Range(int.MinValue, int.MaxValue)
                : roomTypes.generatorRandomizerSeed;
            
            Debug.Log($"Generating using seed: {seed}.");
            
            map = new TileGraph(roomTypes.mapWidth, roomTypes.mapHeight,
                roomTypes.maxRoomsPerBranch, seed);
        }

        Debug.Log($"Map null: {map is null}");
        
        map.roomTypes = roomTypes;
        map.extraHallsChance = roomTypes.extraHallsChance;
        map.specialRoomsChance = roomTypes.specialRoomsChance;
        map.offset = roomTypes.roomOffset;
        map.validDirections = roomTypes.validDirections;
        map.CanLoop = roomTypes.canLoop;
        map.EnableDepthPenalty = roomTypes.mapDepthPenalty;
        map.AttemptBalancing = roomTypes.attemptSpreadBalancing;
        map.MaximumRooms = roomTypes.maxMapRooms;
        map.GenerationChanceBuffer = roomTypes.GenerationChanceBuffer;
        map.algorithm = roomTypes.algorithm;
        map.GenerationChance = roomTypes.generationChance;
        map.GenerationChanceReduction = roomTypes.generationChanceDecay;
        map.MaximumBranchAttempts = roomTypes.maximumBranchAttempts;
        map.roomRules = roomTypes.RoomRules;
        map.baseGenerationChance = roomTypes.baseGenerationChance;
        map.generationChanceIncrease = roomTypes.generationChanceIncrease;
        map.guaranteedGenerationRooms = roomTypes.guaranteedGenerationRooms;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
        
        //Debug.Log(map);

        if(map.StartRoom is not null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            EnemyManager.instance.currentRoom = map.StartRoom;
            map.StartRoom.hasBeenExplored = true;

            // Set camera to new room
            CinemachineConfiner2D cineCam = FindObjectOfType<CinemachineConfiner2D>();

            if (cineCam is not null)
            {
                cineCam.m_BoundingShape2D = map.StartRoom.GetComponent<PolygonCollider2D>();
                cineCam.InvalidateCache();
            }

            player.transform.position = new Vector3(map.StartRoom.gameObject.transform.position.x, 
                map.StartRoom.gameObject.transform.position.y, player.transform.position.z);
            
            List<Room> rooms = new List<Room>(FindObjectsByType<Room>(FindObjectsSortMode.None));

            foreach (Room room in rooms)
            {
                if (room.levelData == null)
                    room.levelData = roomTypes;
            }
        }
    }

    // NOTE: This is very unoptimized -- a better solution would be to generate the navmesh ONLY in the room the player is currently in
    void GenerateNavMesh()
    {
        GridGraph dungeonNavMesh = AstarPath.active.data.gridGraph;
        dungeonNavMesh.RelocateNodes(center: map.StartRoom.transform.position, rotation: Quaternion.identity, nodeSize: dungeonNavMesh.nodeSize);

        dungeonNavMesh.rotation = new Vector3(90, 0, 0);
        dungeonNavMesh.SetDimensions(width: roomTypes.mapWidth * roomTypes.roomOffset * 2 + roomTypes.roomOffset,
            depth: roomTypes.mapHeight * roomTypes.roomOffset * 2 + roomTypes.roomOffset, dungeonNavMesh.nodeSize);

        AstarPath.active.Scan();
    }
}