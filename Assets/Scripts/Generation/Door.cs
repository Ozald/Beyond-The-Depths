using System.Collections;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

public class Door : Connectable
{
    [HideInInspector]
    [CanBeNull] public Door connectedDoor;
    
    [Header("Exit")]
    public float exitOffset;
    public Vector3 exitDirection;

    [HideInInspector]
    public Room parentRoom;
    
    [HideInInspector]
    public bool enabled = false;
    
    [Header("Fade")]
    public Animator fadeAnimator;

    // There was a bug that cause the door transitions to loop, this fixes that
    [HideInInspector]
    public bool isTransitioning;

    void Start()
    {
        fadeAnimator = Fade.instance.GetComponent<Animator>();
        isTransitioning = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && enabled)
        {
            Debug.Log("Player has entered a door");
            
            if (connectedDoor is not null && !isTransitioning)
            {
                StartCoroutine(FadeTransition(other));
            }
        }
    }

    private IEnumerator FadeTransition(Collider2D other)
    {
        isTransitioning = true;
        PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
        
        if(playerMovement is not null)
            playerMovement.canMove = false;
        
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(0.75f);
        
        // I have to do this, otherwise the player renders behind rooms, halls, and doors
        // Note: This caused a bug that cause door transitions to loop indefinitely, so this line of code has been merged into the transforms below
            // other.transform.position = new Vector3(0, 0, other.transform.position.z);

        PlayerManager.instance.currentRoom = connectedDoor.parentRoom;
        
        yield return new WaitForSecondsRealtime(0.2f);
        fadeAnimator.SetTrigger("FadeIn");

        /*
        if (connectedDoor.transform.position.x > transform.position.x)
            other.gameObject.transform.position = new Vector3(connectedDoor.transform.position.x + exitOffset, connectedDoor.transform.position.y, other.transform.position.z);
                
        else if (connectedDoor.transform.position.y > transform.position.y)
            other.gameObject.transform.position = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y + exitOffset, other.transform.position.z);

        else if (connectedDoor.transform.position.y < transform.position.y)
            other.gameObject.transform.position = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y - exitOffset, other.transform.position.z);

        else if (connectedDoor.transform.position.x < transform.position.x)
            other.gameObject.transform.position = new Vector3(connectedDoor.transform.position.x - exitOffset, connectedDoor.transform.position.y, other.transform.position.z);
        */

        // Replaced the system above with an easier, universal system
        // The only thing you would need now is to make sure the doors are facing the right direction
        // We aren't accounting for the z-position since we are going to be using sorting layers

        // Set camera to new room
        CinemachineConfiner2D cineCam = FindObjectOfType<CinemachineConfiner2D>();

        if (cineCam is not null)
        {
            cineCam.m_BoundingShape2D = connectedDoor.parentRoom.GetComponent<PolygonCollider2D>();
            cineCam.InvalidateCache();
            cineCam.GetComponent<CinemachineVirtualCamera>().PreviousStateIsValid = false;
        }

        other.gameObject.transform.position = connectedDoor.transform.position + (exitOffset * -exitDirection);

        yield return new WaitForSecondsRealtime(0.2f);
        if(playerMovement is not null)
            playerMovement.canMove = true;

        isTransitioning = false;
    }
}
