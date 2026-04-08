using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/DefenseUpgrade", order = 1)]
public class DefenseUpgradeStats : UpgradeStats
{
    public int defense;
    
    public override void Apply(StatsManager manager)
    {
        manager.defense.Update(defense);
    }
}