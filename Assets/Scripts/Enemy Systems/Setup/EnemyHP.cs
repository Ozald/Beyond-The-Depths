using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyData enemyData;

    [SerializeField] private int currentHP;
    private float timeSinceLastHit;

    void Start()
    {
        currentHP = enemyData.HP;
    }

    void Update()
    {
        if (Time.timeScale > 0)
            timeSinceLastHit += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") && timeSinceLastHit > enemyData.invincibilityCooldown)
        {
            timeSinceLastHit = 0;
            TakeDamage(damage: 1);
        }
    }

    /********************************************************************/

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Destroy(gameObject);
    }
}
