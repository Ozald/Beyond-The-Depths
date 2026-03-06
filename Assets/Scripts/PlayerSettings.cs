using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Player Settings")]

public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    public float speedVariable;
}
