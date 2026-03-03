using UnityEngine;

public class Door : MonoBehaviour
{
    public Door connectedDoor;
    public BoxCollider2D boxCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name + " entered the door.");
    }
}
