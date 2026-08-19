using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public float cooldown;
    public float attackLifetime;
    public float damage;
    public float knockback;

    public abstract void Attack(GameObject player);
}