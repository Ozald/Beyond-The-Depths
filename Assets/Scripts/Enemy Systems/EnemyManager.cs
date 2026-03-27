using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public Room currentRoom;
    public List<Enemy> enemies;

    void Awake()
    {
        instance = this;
        enemies = new List<Enemy>();
    }

    // DEBUG FEATURE
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X))
            KillAllEnemies();
        #endif
    }

    private void KillAllEnemies()
    {
        foreach (Enemy enemy in enemies)
            enemy.GetComponent<EnemyHP>().TakeDamage(2147483647);
    }

    public bool AllEnemiesDead()
    {
        return enemies.Count == 0;
    }
}
