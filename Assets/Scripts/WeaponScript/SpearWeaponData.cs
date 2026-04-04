using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Spear")]
public class SpearWeaponData : WeaponData
{
    public override void Attack(GameObject player)
    {
        Transform weaponTransform = player.transform.GetComponentInChildren<Weapon>().transform;
        Transform hitboxTransform = weaponTransform.Find("SpearHitbox");
        Collider2D spearHitbox = hitboxTransform.GetComponent<Collider2D>();

        if (spearHitbox != null)
        {

            spearHitbox.gameObject.SetActive(true);
            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(spearHitbox, attackLifetime));

            
        }
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.gameObject.SetActive(false);
    }

}
