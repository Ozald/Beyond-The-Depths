using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public static EnemyHP instance;
    public int maxHP;

    [SerializeField] private int currentHP;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Destroy(gameObject);
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
        #endif
    }
}
