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
            Destroy(room.gameObject);
        
        TileGraph map = new TileGraph(mapWidth, mapHeight, maxRoomsPerBranch);
        
        map.extraHallsChance = extraHallsChance;
        map.specialRoomsChance = specialRoomsChance;
        map.roomTypes = roomTypes;
        map.offset = roomOffset;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
        
        Debug.Log(map);

        if (map.StartRoom is not null)
        {
            PlayerManager.instance.currentRoom = map.StartRoom;
            
            player.transform.position = new Vector3(map.StartRoom.gameObject.transform.position.x, 
                map.StartRoom.gameObject.transform.position.y, player.transform.position.z);
        }
    }
}