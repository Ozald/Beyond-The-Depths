using JetBrains.Annotations;
using UnityEngine;

public class Door : Connectable
{
    [CanBeNull] public Door connectedDoor;
    public BoxCollider2D boxCollider;
    public float exitOffset;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has entered a door");
            
            if (connectedDoor is not null)
            {
                other.transform.position = connectedDoor.transform.position;

                if (connectedDoor.transform.position.x > transform.position.x)
                {
                    other.transform.position += new Vector3(exitOffset, 0, 0);
                }
                
                if (connectedDoor.transform.position.y > transform.position.y)
                {
                    other.transform.position += new Vector3(0, exitOffset, 0);
                }

                if (connectedDoor.transform.position.y < transform.position.y)
                {
                    other.transform.position += new Vector3(0, -exitOffset, 0);
                }

                if (connectedDoor.transform.position.x < transform.position.x)
                {
                    other.transform.position += new Vector3(-exitOffset, 0, 0);
                }
            }
        }
    }
}
