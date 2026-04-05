using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/SpeedUpgrade", order = 1)]
public class SpeedUpgradeStats : UpgradeStats
{
    public int extraSpeed;
    
    public override void Apply(StatsManager manager)
    {
        manager.bonusSpeed += extraSpeed;
        manager.UpdateSpeed();
    }
}