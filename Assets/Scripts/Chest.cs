using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public List<Weapon> weaponList = new List<Weapon>();
    public bool isOpen = false;
    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen)
        {
            isOpen = true;
            Destroy(this);

            Destroy(transform.GetChild(0).gameObject);

            //this.gameObject.SetActive(false);
            int randomIndex = Random.Range(0, weaponList.Count);
            Instantiate(weaponList[randomIndex], this.gameObject.transform.position, this.gameObject.transform.rotation);
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
