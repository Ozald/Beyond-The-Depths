using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public Room currentRoom;
    public int enemyCount = 0;

    void Awake()
    {
        instance = this;
    }

    public bool AllEnemiesDead()
    {
        return enemyCount == 0;
    }
}
