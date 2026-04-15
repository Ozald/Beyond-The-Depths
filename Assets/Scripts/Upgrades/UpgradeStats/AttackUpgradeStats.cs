using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/DamageUpgrade", order = 1)]
public class AttackUpgradeStats : UpgradeStats
{
    public int extraDamage;
    
    public override void Apply(StatsManager manager)
    {
        manager.bonusDamage.Update(extraDamage);
    }
}