using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public float cooldown;
    public float attackSpeed;
    public abstract void Attack(GameObject player);
}