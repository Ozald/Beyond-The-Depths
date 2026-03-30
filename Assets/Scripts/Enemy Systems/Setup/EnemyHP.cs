using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyData enemyData;

    [SerializeField] public int currentHP;
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
        if (collision.gameObject.CompareTag("Bullet") && timeSinceLastHit > enemyData.invincibilityCooldown)
        {
            timeSinceLastHit = 0;
            TakeDamage(damage: 1);
            Destroy(collision.gameObject);
            return;
        }

        if ((collision.gameObject.CompareTag("Player") && timeSinceLastHit > enemyData.invincibilityCooldown))
        {
            // Find the active child of the player
            foreach (Transform child in collision.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    // Check if any of its grandchildren are active
                    foreach (Transform grandchild in child)
                    {
                            if (grandchild.gameObject.activeInHierarchy)
                            {
                                timeSinceLastHit = 0;
                                TakeDamage(damage: 1);
                                return;
                            }
                    }
                }
            }
        }
    }

    /********************************************************************/

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //EnemyManager.instance.enemies.Remove(gameObject.GetComponent<Enemy>());
        Debug.Log(gameObject.name + " died.");
        EnemyManager.instance.enemyCount--;
        Destroy(gameObject);
    }
}
