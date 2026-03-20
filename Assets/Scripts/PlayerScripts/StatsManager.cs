using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Health")]
    public int maxHP;
    public int currentHP;

    [Header("CashMoneyFlow")]
    public int doubloons;

    [Header("Extra")]
    public float invincibilityCooldown;

    private float timeSinceLastHit;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHP = maxHP;
        timeSinceLastHit = 0f;
    }

    void Update()
    {
        timeSinceLastHit += Time.deltaTime;
    }

    public static void TakeDamage(int damage)
    {
        Instance.currentHP -= damage;
        if (Instance.currentHP <= 0)
        {
            Debug.Log("PLAYER HAS DIED");
            Destroy(Instance.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyHurtbox") && timeSinceLastHit > invincibilityCooldown)
            TakeDamage(1);
    }
}
