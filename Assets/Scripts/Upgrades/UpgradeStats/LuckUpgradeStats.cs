using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/LuckUpgrade", order = 1)]
public class LuckUpgradeStats : UpgradeStats
{
    public int extraLuck;
    public float extraCritChance;
    
    public override void Apply(StatsManager manager)
    {
        manager.luck.Update(extraLuck);
        manager.critChance.Update(extraCritChance);
    }
}
