using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/HealthUpgrade", order = 1)]
public class HealthUpgradeStats : UpgradeStats
{
    public int extraHP;
    
    public override void Apply(StatsManager manager)
    {
        manager.bonusMaxHP += extraHP;
        manager.UpdateMaxHP();
    }
}
