using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Sword")]
public class SwordWeaponData : WeaponData
{
    public override void Attack(GameObject player)
    {
        Transform weaponTransform = player.transform.GetComponentInChildren<Weapon>().transform;
        Transform hitboxTransform = weaponTransform.Find("SwordHitbox");
        Collider2D swordHitbox = hitboxTransform.GetComponent<Collider2D>();

        if (swordHitbox != null)
        {
            swordHitbox.gameObject.SetActive(true);
            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(swordHitbox, attackLifetime));
            
        }
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.gameObject.SetActive(false);
    }

}
