using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 2f, player.transform.position.z - 10);
    }
}
