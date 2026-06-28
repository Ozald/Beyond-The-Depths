using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "Weapon Data/Ranged Weapon")]

public class RangedWeaponData : WeaponData
{
    public float bulletSpeed;
    public GameObject bulletPrefab;

    public override void Attack(GameObject player)
    {
        Transform weaponTransform = PlayerInventory.instance.playerInv[0].transform;
        Transform firePoint = weaponTransform.Find("Fire Point");

        if (firePoint != null && bulletPrefab != null)
        {
            AttackHitboxData projectile = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<AttackHitboxData>();
            projectile.speed = bulletSpeed;
            projectile.direction = new Vector2(firePoint.right.x, firePoint.right.y).normalized;
            projectile.maxLifetime = attackLifetime;
            projectile.damage = damage + player.GetComponent<StatsManager>().damage.value;
            
            if (new Random().NextDouble() < player.GetComponent<StatsManager>().critChance.value)
                projectile.damage *= 2;
            
            projectile.knockback = knockback;
            projectile.destroyOnHit = true;
            projectile.tagToHit = "Enemy";

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = firePoint.right * bulletSpeed;
            }

            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(projectile.GetComponent<Collider2D>(), attackLifetime));

            Destroy(projectile.gameObject, attackLifetime);
        }
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
            hitbox.gameObject.SetActive(false);
    }
}
