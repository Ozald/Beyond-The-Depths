using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Serializable]
public struct IntegerStat
{
    public int baseValue;
    public int bonusValue;
    public int value;

    public void Update(int increment)
    {
        bonusValue += increment;
        value = baseValue + bonusValue;
    }
}

[Serializable]
public struct FloatStat
{
    public float baseValue;
    public float bonusValue;
    public float value;

    public void Update(float increment)
    {
        bonusValue += increment;
        value = baseValue + bonusValue;
    }
}

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public Animator fadeAnimator;

    [FormerlySerializedAs("health")] [Header("Health")] 
    public IntegerStat maxHealth;
    
    public int currentHP;
    
    [Header("Speed")] 
    public FloatStat speed;

    [Header("Damage")] 
    public IntegerStat bonusDamage;

    [Header("Crit Chance")]
    public FloatStat critChance;

    [Header("Attack Speed")] 
    public FloatStat bonusAttackSpeed;

    [Header("Defense")]
    public IntegerStat defense;

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
        maxHealth.Update(0);
        speed.Update(0);
        bonusDamage.Update(0);
        defense.Update(0);
        
        currentHP = maxHealth.value;
        timeSinceLastHit = 0f;

        fadeAnimator = Fade.instance.GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.timeScale > 0)
            timeSinceLastHit += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (Instance.timeSinceLastHit < Instance.invincibilityCooldown)
            return;

        Instance.timeSinceLastHit = 0f;

        CameraShake.ShakeCamera(amplitude: 3, duration: 0.2f, isImpactFrame: true);
        Instance.damageEffect.Play();
        AudioManager.PlayOneShot(AudioManager.instance.audioData.playerDamageTaken, delay: 0.2f);
        Instance.currentHP -= damage;

        if (Instance.currentHP <= 0)
        {
            Debug.Log("PLAYER HAS DIED");
            Die();
        }
        else
        {
            Instance.StopCoroutine(Instance.InvincFrameVisualizer());
            Instance.StartCoroutine(Instance.InvincFrameVisualizer());
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("DeathScreen");
        Debug.Log("Returned to menu");
        yield return new WaitForSecondsRealtime(0.2f);
    }

    // Attack Damage
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyHurtbox"))
        {
            AttackHitboxData projectile = collision.GetComponent<AttackHitboxData>();

            if (projectile != null)
                TakeDamage(damage: projectile.damage);
            else
                TakeDamage(1);
        }
    }

    // Contact Damage
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(damage: 1);
        }
    }

    private IEnumerator InvincFrameVisualizer()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // SpriteRenderer shadow = GetComponent<DropShadow>().currentShadow.GetComponent<SpriteRenderer>();

        yield return new WaitForSeconds(invincibilityCooldown * 0.1f);
        Color currColor = spriteRenderer.color;
        spriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 0.8f);

        yield return new WaitForSeconds(invincibilityCooldown * 0.9f);

        spriteRenderer.color = currColor;
    }
}
