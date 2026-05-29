using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public List<Weapon> weaponList = new List<Weapon>();
    public bool isOpen = false;
    public Animator animator;

    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen)
        {
            isOpen = true;

            this.enabled = false;

            animator.SetBool("IsOpen", true);

            StartCoroutine(OpenChestRoutine());
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        yield return new WaitForSeconds(0.40f);

        int randomIndex = Random.Range(0, weaponList.Count);

        Instantiate(
            weaponList[randomIndex], transform.position, transform.rotation);
        Destroy(this);
    }
}