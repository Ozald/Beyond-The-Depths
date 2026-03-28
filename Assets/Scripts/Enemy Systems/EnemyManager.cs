using System.Collections.Generic;
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

    public bool AllEnemiesDead()
    {
        return enemies.Count == 0;
    }
}
