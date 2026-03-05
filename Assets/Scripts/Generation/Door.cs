using JetBrains.Annotations;
using UnityEngine;

public class Door : Connectable
{
    [CanBeNull] public Door connectedDoor;
    public BoxCollider2D boxCollider;
}
