using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/CritChanceUpgrade", order = 1)]
public class CritChanceUpgradeStats : UpgradeStats
{
    public float extraCritChance;
    
    public override void Apply(StatsManager manager)
    {
        manager.critChance.Update(extraCritChance);
    }
}