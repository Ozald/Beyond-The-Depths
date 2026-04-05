using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Health")] 
    public int baseMaxHP;

    public int bonusMaxHP;
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
        maxHP = baseMaxHP + bonusMaxHP;
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
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance.StopCoroutine(Instance.InvincFrameVisualizer());
            Instance.StartCoroutine(Instance.InvincFrameVisualizer());
        }
    }

    public void UpdateMaxHP()
    {
        maxHP = baseMaxHP + bonusMaxHP;
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
