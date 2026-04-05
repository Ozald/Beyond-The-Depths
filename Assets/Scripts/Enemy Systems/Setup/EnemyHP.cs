using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyData enemyData;
    public Material flashMaterial;

    [SerializeField] public int currentHP;
    private float timeSinceLastHit;

    void Start()
    {
        currentHP = enemyData.HP;
        timeSinceLastHit = enemyData.invincibilityCooldown;
    }

    void Update()
    {
        if (Time.timeScale > 0)
            timeSinceLastHit += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject, 0.05f);

            if (timeSinceLastHit < enemyData.invincibilityCooldown)
                return;

            timeSinceLastHit = 0;
            AttackHitboxData projectile = collision.GetComponent<AttackHitboxData>();
            TakeDamage(damage: projectile.damage, knockback: projectile.knockback);
            return;
        }

        /*
        if ((collision.gameObject.CompareTag("Player") && timeSinceLastHit > enemyData.invincibilityCooldown))
        {
            // Find the active child of the player
            foreach (Transform child in collision.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    Weapon weapon = child.GetComponent<Weapon>();
                    // Check if any of its grandchildren are active
                    foreach (Transform grandchild in child)
                    {
                        if (weapon != null && weapon.weaponData != null)
                        {
                            timeSinceLastHit = 0;

                            // Use damage from WeaponData
                            int damageAmount = weapon.weaponData.damage;
                            TakeDamage(damage: damageAmount);

                            return;
                        }
                    }
                }
            }
        }*/

        if (collision.gameObject.CompareTag("Weapon") && timeSinceLastHit > enemyData.invincibilityCooldown)
        {
            timeSinceLastHit = 0;
            AttackHitboxData attack = collision.GetComponent<AttackHitboxData>();
            TakeDamage(attack.damage, attack.knockback);
            return;
        }
    }

    /********************************************************************/

    public void TakeDamage(int damage, int knockback)
    {
        currentHP -= damage;
        StartCoroutine(FlashEnemy());

        Vector3 knockbackDir = -(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = knockbackDir * knockback;

        AudioManager.PlayOneShot(AudioManager.GetAudioData().enemyDamageTaken);

        if (currentHP <= 0)
        {
            CameraShake.ShakeCamera(amplitude: 2f, duration: 0.3f, isImpactFrame: true);
            AudioManager.PlayOneShot(sound: AudioManager.GetAudioData().enemyDeath, delay: 0.3f);
            Die();
        }
        else
        {
            StopCoroutine(InvincFrameVisualizer());
            StartCoroutine(InvincFrameVisualizer());
        }
    }

    public IEnumerator FlashEnemy()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Material originalMaterial = spriteRenderer.material;

        // Now do the flash

        spriteRenderer.color = Color.white;
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
        spriteRenderer.material = originalMaterial;
    }

    private void Die()
    {
        //EnemyManager.instance.enemies.Remove(gameObject.GetComponent<Enemy>());
        Debug.Log(gameObject.name + " died.");

        if (EnemyManager.instance.enemyCount > 0)
            EnemyManager.instance.enemyCount--;

        GetComponent<Enemy>().enabled = false;

        Vector3 knockbackDir = -(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = false;
        rb.velocity = knockbackDir * 15f;
        rb.AddTorque(100f);

        Destroy(gameObject, 0.7f);
    }

    private IEnumerator InvincFrameVisualizer()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // SpriteRenderer shadow = GetComponent<DropShadow>().currentShadow.GetComponent<SpriteRenderer>();

        yield return new WaitForSeconds(enemyData.invincibilityCooldown * 0.1f);
        Color currColor = spriteRenderer.color;
        spriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 0.8f);

        yield return new WaitForSeconds(enemyData.invincibilityCooldown * 0.9f);

        spriteRenderer.color = currColor;
    }
}
