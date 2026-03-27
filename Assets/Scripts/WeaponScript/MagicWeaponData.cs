using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeaponData : WeaponData
{
    public override void Attack(GameObject player)
    {
        

        Transform weaponTransform = player.transform.Find("Sword");
        Transform hitboxTransform = weaponTransform.Find("SwordHitbox");
        Collider2D bulletHitbox = hitboxTransform.GetComponent<Collider2D>();

        if (bulletHitbox != null)
        {
            //bulletHitbox.gameObject.SetActive(true);

            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(bulletHitbox, attackLifetime));
        }
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.gameObject.SetActive(false);
    }
}
