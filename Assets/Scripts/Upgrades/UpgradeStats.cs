using UnityEngine;

public abstract class UpgradeStats : ScriptableObject
{
    public abstract void Apply(StatsManager manager);
}
