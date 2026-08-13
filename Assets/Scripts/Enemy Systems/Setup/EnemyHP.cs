using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyData enemyData;
    public Material flashMaterial;
    public GameObject HP_Drop;

    [SerializeField] public float currentHP;
    public float timeSinceLastHit { private set; get; }

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

    /********************************************************************/

    public void TakeDamage(float damage, float knockback)
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

        timeSinceLastHit = 0;
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

        Enemy enemy = GetComponent<Enemy>();

        if (enemy != null)
            enemy.enabled = false;

        Vector3 knockbackDir = -(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = false;
        rb.velocity = knockbackDir * 15f;
        rb.angularVelocity = 1000f;

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        float dropChance = Random.value;

        if (dropChance <= 0.5)
        {
            Instantiate(HP_Drop, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject, 0.7f);
    }
}
