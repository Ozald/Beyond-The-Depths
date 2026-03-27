using Cinemachine;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public LevelData roomTypes;

    public int mapWidth;
    public int mapHeight;
    public int maxRoomsPerBranch;
    public float extraHallsChance;
    public float specialRoomsChance;
    public int roomOffset;

    public TileGraph map;
    
    void Start()
    {
        map = new TileGraph(mapWidth, mapHeight, maxRoomsPerBranch);
        
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

    void Update()
    {   
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GenerateFloorLayout();
        }
        #endif
    }

    void GenerateFloorLayout()
    {
        List<Connectable> roomCollection = new List<Connectable>(FindObjectsByType<Connectable>(FindObjectsSortMode.None));
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        foreach (Connectable room in roomCollection)
            Destroy(room.gameObject);
        
        if(map is null)
            map = new TileGraph(mapWidth, mapHeight, maxRoomsPerBranch);
        
        Debug.Log($"Map null: {map is null}");
        
        map.extraHallsChance = extraHallsChance;
        map.specialRoomsChance = specialRoomsChance;
        map.roomTypes = roomTypes;
        map.offset = roomOffset;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
        
        Debug.Log(map);

        if (map.StartRoom is not null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            EnemyManager.instance.currentRoom = map.StartRoom;
            
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
        dungeonNavMesh.SetDimensions(width: mapWidth * roomOffset * 2, depth: mapHeight * roomOffset * 2, dungeonNavMesh.nodeSize);

        AstarPath.active.Scan();
    }
}