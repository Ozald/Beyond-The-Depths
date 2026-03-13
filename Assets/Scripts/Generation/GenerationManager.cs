using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public Connectable roomPrefab;
    public Connectable hallPrefab;
    public Connectable doorPrefab;

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

        map.GenerateMap(new(map.Width / 2, map.Height / 2));

        if (map.StartRoom is not null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            
            player.transform.position = new Vector3(map.StartRoom.gameObject.transform.position.x * 5, 
                map.StartRoom.gameObject.transform.position.y * 5, player.transform.position.z);
        }
    }
}