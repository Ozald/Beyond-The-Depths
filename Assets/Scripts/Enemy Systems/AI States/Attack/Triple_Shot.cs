using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]

public class Triple_Shot : AIState
{
    public AttackHitboxData projectilePrefab;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 5f;
    public float spreadInDegrees = 45f;
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
            enemy.StartCoroutine(SpawnProjectiles(enemy));
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

    public override void OnFixedUpdate(Enemy enemy) { }
    public override void OnExit(Enemy enemy) { }

    /****************************************************************************/

    // Note: This causes a bug where an enemy can spawn projectiles ever after it's dead if it dies during the attack animation, so this should be changed in the future.
    IEnumerator SpawnProjectiles(Enemy enemy)
    {
        yield return new WaitForSeconds(0.5f);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 playerDir = (player.position - enemy.transform.position).normalized;

        Vector3[] attackDirs =
        {
            playerDir,
            Quaternion.Euler(0f, 0f, -spreadInDegrees) * playerDir,
            Quaternion.Euler(0f, 0f, spreadInDegrees) * playerDir
        };

        foreach (Vector3 attackDir in attackDirs)
        {
            if (!enemy.enabled)
                yield break;
            
            AttackHitboxData projectile = Object.Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
            projectile.speed = projectileSpeed;
            projectile.direction = new Vector2(attackDir.x, attackDir.y);
            projectile.maxLifetime = projectileLifetime;
            projectile.damage = 1;
        }
    }
}

/*******************************************************************/

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Triple_Shot))]
public class TripleShotEditor : PropertyDrawer
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
