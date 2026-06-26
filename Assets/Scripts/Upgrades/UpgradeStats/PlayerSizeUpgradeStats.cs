using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/PlayerSizeUpgrade", order = 1)]
public class PlayerSizeUpgradeStats : UpgradeStats
{
    public float scaleAmount;
    public float bonusSpeed;
    
    public override void Apply(StatsManager manager)
    {
        // Prevent negative scale values
        if (manager.playerSize.value > 0.2)
        {
            manager.playerSize.Update(-scaleAmount);
            
            manager.gameObject.transform.localScale =
                new Vector3(1, 1, 1) - new Vector3(1 - manager.playerSize.value, 1 - manager.playerSize.value,
                    1 - manager.playerSize.value);
        }

        manager.speed.Update(bonusSpeed);
    }
}