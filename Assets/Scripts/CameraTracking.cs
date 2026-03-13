using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    void Start()
    {
        transform.position = PlayerManager.instance.transform.position + new Vector3(0, 0, -10);
    }
    
    void Update()
    {
        Room room = PlayerManager.instance.currentRoom;
        
        transform.position = new Vector3(room.transform.position.x, room.transform.position.y, room.transform.position.z - 10);
    }
}
