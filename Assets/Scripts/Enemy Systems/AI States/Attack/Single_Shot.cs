using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]

public class Single_Shot : AIState
{
    public AttackHitboxData projectilePrefab;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 5f;
    public float spreadInDegrees = 5f;
    public string attackAnimationTrigger;

    public override void OnEnter(Enemy enemy)
    {
        enemy.transform.rotation = Quaternion.identity;

        if (!EnemyAttackManager.instance.RequestAttack(enemy))
            return;

        Animator enemyAnim = enemy.GetComponent<Animator>();
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();

        if (enemyAnim != null)
            enemyAnim.SetTrigger(attackAnimationTrigger);

        if (projectilePrefab != null)
            enemy.StartCoroutine(SpawnProjectile(enemy));
    }

    public override void OnUpdate(Enemy enemy)
    {
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enemyRenderer != null && player != null)
        {
            if (player.position.x > enemy.transform.position.x)
                enemyRenderer.flipX = true;
            else
                enemyRenderer.flipX = false;
        }
    }

    // Unused
    public override void OnFixedUpdate(Enemy enemy) { }
    public override void OnExit(Enemy enemy) { }

    /****************************************************************************/

    // Note: This causes a bug where an enemy can spawn projectiles ever after it's dead if it dies during the attack animation, so this should be changed in the future.
    IEnumerator SpawnProjectile(Enemy enemy)
    {
        yield return new WaitForSeconds(0.5f);
        
        if (!enemy.enabled)
            yield break;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector2 attackDir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
        float angle = Random.Range(-spreadInDegrees, spreadInDegrees);
        
        AttackHitboxData projectile = Object.Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        projectile.speed = projectileSpeed;
        projectile.direction = Quaternion.Euler(0, 0, angle) * attackDir;
        projectile.maxLifetime = projectileLifetime;
        projectile.damage = 1;
    }
}

/*******************************************************************/

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Single_Shot))]
public class SingleShotEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        //public AttackHitboxData projectilePrefab;
        //public float projectileSpeed = 5f;
        //public float projectileLifetime = 5f;
        //public float spreadInDegrees = 45f;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Parameters", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("projectilePrefab"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("projectileSpeed"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("projectileLifetime"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("spreadInDegrees"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("attackAnimationTrigger"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
#endif
