using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPContextProvider : MonoBehaviour
{
    public EnemyHP hpToTrack;
    public string contextKey = "HP_Percentage";

    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (hpToTrack != null && enemy != null)
        {
            enemy.GetData().SetAttribute(contextKey, hpToTrack.currentHP / enemy.enemyData.HP);
        }
    }
}
