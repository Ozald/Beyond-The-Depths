using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public float cooldown;
    public float attackLifetime;
    public int damage;
    public int knockback;
    public int weaponDataID = -1;

    public abstract void Attack(GameObject player);
}