using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Basic_Melee : AIState
{
    public GameObject hitboxObject;
    public string attackAnimationTrigger;

    private const string attackHitboxKey = "ActiveAttackHitbox";

    public override void OnEnter(Enemy enemy)
    {
        if (!enemy.enabled)
            return;
        
        enemy.transform.rotation = Quaternion.identity;

        if (!EnemyAttackManager.instance.RequestAttack(enemy))
            return;

        Animator enemyAnim = enemy.GetComponent<Animator>();
        GameObject attackHitbox = Object.Instantiate(hitboxObject, enemy.transform);

        // Store the active attack hitbox in the enemy's context for later use
        enemy.GetData().SetAttribute(attackHitboxKey, attackHitbox);

        attackHitbox.transform.localPosition = Vector3.zero;
        attackHitbox.transform.localRotation = Quaternion.identity;
        attackHitbox.transform.localScale = Vector3.one;
        attackHitbox.SetActive(false);

        if (enemyAnim != null)
            enemyAnim.SetTrigger(attackAnimationTrigger);
    }

    public override void OnExit(Enemy enemy)
    {
        GameObject attackHitbox = enemy.GetData().GetAttribute<GameObject>(attackHitboxKey);

        if (attackHitbox is not null)
        {
            enemy.GetData().DeleteAttribute(attackHitboxKey);
            Object.Destroy(attackHitbox);
        }
    }

    public override void OnUpdate(Enemy enemy)
    {
        Transform player = PlayerManager.instance.transform;
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        GameObject attackHitbox = enemy.GetData().GetAttribute<GameObject>(attackHitboxKey);

        if (player == null || attackHitbox == null)
            return;

        if (enemy.stateTimer < 0.5)
        {
            Vector3 attackDir = player.position - enemy.transform.position;
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            attackHitbox.transform.position = enemy.transform.position;
            attackHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (enemyRenderer != null)
            {
                if (player.position.x > enemy.transform.position.x)
                    enemyRenderer.flipX = true;
                else
                    enemyRenderer.flipX = false;
            }

            return;
        }

        attackHitbox.SetActive(true);
    }

    // Unused
    public override void OnFixedUpdate(Enemy enemy) {}
}

/*******************************************************************/

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Basic_Melee))]
public class BasicMeleeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Parameters", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("hitboxObject"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("attackAnimationTrigger"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 5 + 10;
    }
}
#endif
