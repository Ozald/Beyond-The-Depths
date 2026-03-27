using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Magic")]

public class MagicWeaponData : WeaponData
{
    public float bulletSpeed;
    public GameObject bulletPrefab;

    public override void Attack(GameObject player)
    {
        Transform weaponTransform = player.transform.Find("Magic");
        Transform firePoint = weaponTransform.Find("Fire Point");

        if (firePoint != null && bulletPrefab != null)
        {
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.up * bulletSpeed;
            }

            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(newBullet.GetComponent<Collider2D>(), attackLifetime));

            Destroy(newBullet, attackLifetime);
        }

        
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
            hitbox.gameObject.SetActive(false);
    }
}
