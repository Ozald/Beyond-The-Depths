using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Sword")]
public class SwordWeaponData : WeaponData
{
    public int damage = 1;
    //public float attackSpeed = 0.3f;
    //public float cooldown = 3f;

    public override void Attack(GameObject player)
    {
        Transform hitboxTransform = player.transform.Find("SwordHitbox");
        Collider2D swordHitbox = hitboxTransform.GetComponent<Collider2D>();

        if (swordHitbox != null)
        {
            swordHitbox.gameObject.SetActive(true);
            player.GetComponent<MonoBehaviour>().StartCoroutine(DisableHitbox(swordHitbox, attackSpeed));
            
        }
    }

    private IEnumerator DisableHitbox(Collider2D hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.gameObject.SetActive(false);
    }

}
