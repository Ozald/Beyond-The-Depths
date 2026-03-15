using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public Connectable roomPrefab;
    public Connectable hallPrefab;
    public Connectable doorPrefab;

    public RoomTypeData roomTypes;

    public int mapWidth;
    public int mapHeight;
    public int maxRoomsPerBranch;
    public float extraHallsChance;
    public int maxSpecialRooms;
    public float specialRoomsChance;
    
    void Start()
    {
        GenerateFloorLayout();
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
        GameObject player = GameObject.Find("TestPlayer");
        
        foreach (Connectable room in roomCollection)
        {
            Destroy(room.gameObject);
        }
        
        TileGraph map = new TileGraph(mapWidth, mapHeight, maxRoomsPerBranch);
        
        map.roomPrefab = roomPrefab;
        map.hallPrefab = hallPrefab;
        map.doorPrefab = doorPrefab;
        map.extraHallsChance = extraHallsChance;
        map.maxSpecialRooms = maxSpecialRooms;
        map.specialRoomsChance = specialRoomsChance;
        map.roomTypes = roomTypes;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
        
        Debug.Log(map);

        if (map.StartRoom != null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            
            player.transform.position = new Vector3(map.StartRoom.gameObject.transform.position.x, 
                map.StartRoom.gameObject.transform.position.y, player.transform.position.z);
        }
    }
}