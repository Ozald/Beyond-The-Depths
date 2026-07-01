using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/KnockbackUpgrade", order = 1)]
public class KnockbackUpgradeStats : UpgradeStats
{
    public float bonusKnockback;

    public override void Apply(StatsManager manager)
    {
        manager.knockback.Update(bonusKnockback);
    }
}
