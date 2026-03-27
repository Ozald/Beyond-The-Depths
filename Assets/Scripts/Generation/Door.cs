using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

public class Door : Connectable
{
    [CanBeNull] public Door connectedDoor;
    public float exitOffset;
    public Room parentRoom;
    public bool enabled = false;
    public Animator fadeAnimator;

    void Start()
    {
        fadeAnimator = Fade.instance.GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && enabled)
        {
            Debug.Log("Player has entered a door");
            
            if (connectedDoor is not null)
            {
                StartCoroutine(FadeTransition(other));
            }
        }
    }

    private IEnumerator FadeTransition(Collider2D other)
    {
        PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
        
        if(playerMovement is not null)
            playerMovement.canMove = false;
        
        fadeAnimator.SetTrigger("Transition");
        yield return new WaitForSecondsRealtime(0.5f);
        
        // I have to do this, otherwise the player renders behind rooms, halls, and doors
        other.transform.position = connectedDoor.transform.position + new Vector3(0, 0, other.transform.position.z);
        PlayerManager.instance.currentRoom = connectedDoor.parentRoom;
                
        if (connectedDoor.transform.position.x > transform.position.x)
            other.transform.position += new Vector3(exitOffset, 0, 0);
                
        if (connectedDoor.transform.position.y > transform.position.y)
            other.transform.position += new Vector3(0, exitOffset, 0);

        if (connectedDoor.transform.position.y < transform.position.y)
            other.transform.position += new Vector3(0, -exitOffset, 0);

        if (connectedDoor.transform.position.x < transform.position.x)
            other.transform.position += new Vector3(-exitOffset, 0, 0);

        // Set camera to new room
        CinemachineConfiner2D cineCam = FindObjectOfType<CinemachineConfiner2D>();

        if (cineCam is not null)
        {
            cineCam.m_BoundingShape2D = connectedDoor.parentRoom.GetComponent<PolygonCollider2D>();
            cineCam.InvalidateCache();
        }
        
        yield return new WaitForSecondsRealtime(0.5f);
        fadeAnimator.SetTrigger("Transition");
        
        yield return new WaitForSecondsRealtime(0.2f);
        if(playerMovement is not null)
            playerMovement.canMove = true;
    }
}
