using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    public static EnemyAttackManager instance;
    public Buffer<Enemy> Enemies;
    public Queue<Enemy> ApprovedEnemies;

    public int MaxAttackingEnemies = 3;

    void Awake()
    {
        instance = this;
        Enemies = new Buffer<Enemy>();
        ApprovedEnemies = new Queue<Enemy>();
    }

    void Start()
    {
        InvokeRepeating("UpdateEnemies", 0, 0.15f);
        InvokeRepeating("ClearApproved", 0, 2f);
    }

    void ClearApproved()
    {
        ApprovedEnemies.Clear();
    }

    void UpdateEnemies()
    {
        if(ApprovedEnemies.Count > 0)
            ApprovedEnemies.Dequeue();

        if (Enemies.Count > 0 && ApprovedEnemies.Count < MaxAttackingEnemies)
        {
            Enemy e = Enemies.Remove();
            
            if(!ApprovedEnemies.Contains(e))
                ApprovedEnemies.Enqueue(e);
        }
        
        Debug.Log(ApprovedEnemies.Count);
    }

    /// <summary>
    /// Allows an enemy to request to attack
    /// </summary>
    /// <param name="enemy">
    /// The enemy requesting
    /// </param>
    /// <returns>
    /// If the enemy should be allowed to attack
    /// </returns>
    public bool RequestAttack(Enemy enemy)
    {
        if (ApprovedEnemies.Contains(enemy))
        {
            ApprovedEnemies.Dequeue();
            return true;
        }

        if (ApprovedEnemies.Count < MaxAttackingEnemies)
        {
            if(!ApprovedEnemies.Contains(enemy))
                ApprovedEnemies.Enqueue(enemy);
            return true;
        }

        Debug.Log("Attack request denied for " + enemy.name);
        return false;
    }
}