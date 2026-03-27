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

    [Header("FX")]
    public ParticleSystem damageEffect;

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
        if (Time.timeScale > 0)
            timeSinceLastHit += Time.deltaTime;
    }

    public static void TakeDamage(int damage)
    {
        CameraShake.ShakeCamera(amplitude: 3, duration: 0.2f, isImpactFrame: true);
        Instance.damageEffect.Play();
        AudioManager.PlayOneShot(AudioManager.instance.audioData.damageTaken, delay: 0.2f);
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
        {
            timeSinceLastHit = 0f;
            TakeDamage(damage: 1);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && timeSinceLastHit > invincibilityCooldown)
        {
            timeSinceLastHit = 0f;
            TakeDamage(damage: 1);
        }
    }
}
