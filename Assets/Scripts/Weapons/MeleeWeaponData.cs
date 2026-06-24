using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "Weapon Data/Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
    public GameObject hitbox;
    
    public override void Attack(GameObject player)
    {
        Transform weaponTransform = PlayerInventory.instance.playerInv[0].transform;

        if (hitbox != null)
        {
            // This caused a hilarious scaling bug that made the sword bigger than the map
            hitbox.transform.localScale = new Vector3(1, 1, 1) * player.GetComponent<StatsManager>().attackRange.value;
            weaponTransform.localScale = hitbox.transform.localScale;
            AttackHitboxData attack = Instantiate(hitbox.gameObject, player.transform.position, PlayerInventory.instance.playerInv[0].transform.rotation * Quaternion.Euler(0, 0, 90)).GetComponentInChildren<AttackHitboxData>();
            if (attack == null)
                return;

            attack.speed = 0;
            attack.direction = Vector3.zero;
            attack.maxLifetime = attackLifetime;
            attack.damage = damage + player.GetComponent<StatsManager>().bonusDamage.value;
            
            if (new Random().NextDouble() < player.GetComponent<StatsManager>().critChance.value)
                attack.damage *= 2;
            
            attack.knockback = knockback;
            attack.destroyOnHit = false;
            attack.tagToHit = "Enemy";
        }
    }
}
