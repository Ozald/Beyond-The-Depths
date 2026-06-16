using System.Collections.Generic;
using UnityEngine;

public class UpgradeInventory : MonoBehaviour
{
    public List<Upgrade> upgrades = new();
    public ParticleSystem particles;

    public void PickupUpgrade(Upgrade nearbyUpgrade)
    {
        upgrades.Add(nearbyUpgrade);
        ParticleSystem particle = Instantiate(particles, transform.position, transform.rotation);
        particle.Play();
    }
}
