using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public class HoveredEvent : UnityEvent<Interactable>
    {
        
    }

    //public HoveredEvent ObjectHovered = new HoveredEvent();

    //FYI, "NonSerialized" just hides the variable from the inspector

    [NonSerialized] public GameObject player;
    private Interactable currentHovered;
    public Interactable objectToInteract = null;
    private Interactable lastInteractable = null;

    private void Start()
    {
        player = gameObject;
    }

    void Update()
    {

        if (objectToInteract != null && objectToInteract != lastInteractable && objectToInteract.CanInteract)
        {
            // Turn ON new outline
            ToggleOutline(objectToInteract, true);

            // Turn OFF old outline
            if (lastInteractable != null)
            {
                ToggleOutline(lastInteractable, false);
            }

            lastInteractable = objectToInteract;
        }

        // Prevent weird things with chests retaining outlines
        if (objectToInteract != null && !objectToInteract.CanInteract)
        {
            ToggleOutline(objectToInteract, false);
            objectToInteract = null;
        }

        if (lastInteractable != null && !lastInteractable.CanInteract)
        {
            ToggleOutline(lastInteractable, false);
            lastInteractable = null;
        }

        // If nothing is in range anymore
        if (objectToInteract == null && lastInteractable != null)
        {
            ToggleOutline(lastInteractable, false);
            lastInteractable = null;
        }

        // If an Interactable object was found, interact with it when you press the Interact key
        if (Input.GetKeyDown(KeyCode.E) && objectToInteract != null && objectToInteract.CanInteract)
        {
            Debug.Log("Interactable detected as " + objectToInteract.name + ", trying interaction");
            objectToInteract.Interact(player.GetComponent<PlayerInteraction>());
            
            if(objectToInteract != null && !objectToInteract.CanInteract)
                ToggleOutline(objectToInteract, false);
            
            if(lastInteractable != null && !lastInteractable.CanInteract)
                ToggleOutline(lastInteractable, false);
            
            objectToInteract = null;
        }
    }

    void ToggleOutline(Interactable interactable, bool state)
    {
        if (interactable == null) return;

        var outline = interactable.GetComponent<SimpleOutline>();

        if (state)
        {
            if (outline == null)
                interactable.gameObject.AddComponent<SimpleOutline>();
        }
        else
        {
            if (outline != null)
                Destroy(outline);
        }
    }

    bool isInTriggerZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();

        if (interactable != null && interactable != objectToInteract && (PlayerInventory.instance.playerInv.Count == 0 || interactable != PlayerInventory.instance.playerInv[0]))
        {
            objectToInteract = interactable;
            isInTriggerZone = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();

        if (objectToInteract != null)
            return;

        if (interactable != null && interactable != objectToInteract && (PlayerInventory.instance.playerInv.Count == 0 || interactable != PlayerInventory.instance.playerInv[0]))
        {
            objectToInteract = interactable;
            isInTriggerZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectToInteract = collision.gameObject.GetComponent<Interactable>();

        if (objectToInteract != null)
        {
            isInTriggerZone = false;
            objectToInteract = null;
        }
    }

}

