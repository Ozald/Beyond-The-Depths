using Cinemachine;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public LevelData roomTypes;

    public TileGraph map;
    
    void Start()
    {
        map = new TileGraph(roomTypes.mapWidth, roomTypes.mapHeight, 
            roomTypes.maxRoomsPerBranch, roomTypes.GeneratorRandomizerSeed);
        
        GenerateFloorLayout();
        GenerateNavMesh();
        
        // Set camera to new room
        CinemachineConfiner2D cineCam = FindObjectOfType<CinemachineConfiner2D>();

        if (cineCam is not null)
        {
            cineCam.m_BoundingShape2D = map.StartRoom.GetComponent<PolygonCollider2D>();
            cineCam.InvalidateCache();
        }
    }

    void GenerateFloorLayout()
    {
        List<Connectable> roomCollection = new List<Connectable>(FindObjectsByType<Connectable>(FindObjectsSortMode.None));
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        foreach (Connectable room in roomCollection)
            Destroy(room.gameObject);
        
        if(map is null)
            map = new TileGraph(roomTypes.mapWidth, roomTypes.mapHeight, 
                roomTypes.maxRoomsPerBranch, roomTypes.GeneratorRandomizerSeed);
        
        Debug.Log($"Map null: {map is null}");
        
        map.roomTypes = roomTypes;
        map.extraHallsChance = roomTypes.extraHallsChance;
        map.specialRoomsChance = roomTypes.specialRoomsChance;
        map.offset = roomTypes.roomOffset;
        map.validDirections = roomTypes.validDirections;
        map.CanLoop = roomTypes.CanLoop;
        map.EnableDepthPenalty = roomTypes.MapDepthPenalty;
        map.AttemptBalancing = roomTypes.AttemptSpreadBalancing;
        map.MaximumRooms = roomTypes.maxMapRooms;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
        
        Debug.Log(map);

        if(map.StartRoom is not null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            EnemyManager.instance.currentRoom = map.StartRoom;
            map.StartRoom.hasBeenExplored = true;
            
            player.transform.position = new Vector3(map.StartRoom.gameObject.transform.position.x, 
                map.StartRoom.gameObject.transform.position.y, player.transform.position.z);
        }
    }

    // NOTE: This is very unoptimized -- a better solution would be to generate the navmesh ONLY in the room the player is currently in
    void GenerateNavMesh()
    {
        GridGraph dungeonNavMesh = AstarPath.active.data.gridGraph;
        dungeonNavMesh.RelocateNodes(center: map.StartRoom.transform.position, rotation: Quaternion.identity, nodeSize: dungeonNavMesh.nodeSize);

        dungeonNavMesh.rotation = new Vector3(90, 0, 0);
        dungeonNavMesh.SetDimensions(width: roomTypes.mapWidth * roomTypes.roomOffset * 2,
            depth: roomTypes.mapHeight * roomTypes.roomOffset * 2, dungeonNavMesh.nodeSize);

        AstarPath.active.Scan();
    }
}