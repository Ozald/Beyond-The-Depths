using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/AttackRangeUpgrade", order = 1)]
public class AttackRangeUpgradeStats : UpgradeStats
{
    public float bonusRange;

    public override void Apply(StatsManager manager)
    {
        manager.attackRange.Update(bonusRange);
    }
}
