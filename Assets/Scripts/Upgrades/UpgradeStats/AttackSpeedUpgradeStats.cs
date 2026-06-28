using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Stats", menuName = "ScriptableObjects/UpgradeStats/AttackSpeedUpgrade", order = 1)]
public class AttackSpeedUpgradeStats : UpgradeStats
{
    public float extraAttackSpeed;
    
    public override void Apply(StatsManager manager)
    {
        manager.attackSpeed.Update(extraAttackSpeed);
    }
}