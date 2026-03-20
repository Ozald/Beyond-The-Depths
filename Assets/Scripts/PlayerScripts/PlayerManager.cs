using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Room currentRoom;
    
    void Awake()
    {
        if (instance is null)
            instance = this;
    }
}
