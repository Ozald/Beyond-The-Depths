using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Default Weapon")]
public class DefaultWeapon : ScriptableObject
{
    [Header("General")]

    public float attackSpeed;
    public int weaponDamage;

}
