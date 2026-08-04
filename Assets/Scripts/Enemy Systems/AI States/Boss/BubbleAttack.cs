using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BubbleAttack : AIState
{
    public GameObject bubblePrefab;
    public float projectileForce = 10f;

    public override void OnEnter(Enemy enemy)
    {
        SpriteRenderer spriteRenderer = GameObject.FindGameObjectWithTag("Chud").GetComponent<SpriteRenderer>();
        Collider2D collider = GameObject.FindGameObjectWithTag("Chud").GetComponent<Collider2D>();

        if (spriteRenderer is not null)
            spriteRenderer.enabled = false;
        if (collider is not null)
            collider.enabled = false;


        if (!enemy.GetData().HasAttribute("IsAttacking") || !enemy.GetData().GetAttribute<bool>("IsAttacking"))
        {
            Debug.Log("Starting Bubble Attack");
            enemy.StartCoroutine(Attack(enemy));
            enemy.GetData().SetAttribute("IsAttacking", true);
        }
    }

    public override void OnExit(Enemy enemy)
    {
        SpriteRenderer spriteRenderer = GameObject.FindGameObjectWithTag("Chud").GetComponent<SpriteRenderer>();
        Collider2D collider = GameObject.FindGameObjectWithTag("Chud").GetComponent<Collider2D>();

        if (spriteRenderer is not null)
            spriteRenderer.enabled = false;
        if (collider is not null)
            collider.enabled = false;

        enemy.GetData().SetAttribute("IsAttacking", false);
    }

    public override void OnFixedUpdate(Enemy enemy)
    {
        
    }

    public override void OnUpdate(Enemy enemy)
    {
        
    }

    private IEnumerator Attack(Enemy enemy)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 4; i++)
        {
            GameObject bubble = Object.Instantiate(bubblePrefab, enemy.transform.position, Quaternion.identity);

            Vector2 attackDir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
            bubble.GetComponent<Rigidbody2D>().AddForce(attackDir * projectileForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(1f);
        }

        Rigidbody2D chud = GameObject.FindGameObjectWithTag("Chud").GetComponent<Rigidbody2D>();

        if (chud is not null)
        {
            chud.transform.position = enemy.transform.position;
            chud.GetComponent<SpriteRenderer>().enabled = true;
            chud.GetComponent<Collider2D>().enabled = true;

            Vector2 attackDir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
            chud.AddForce(attackDir * projectileForce * 2, ForceMode2D.Impulse);
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BubbleAttack))]
public class BubbleAttackEditor : PropertyDrawer
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
        EditorGUI.PropertyField(r, property.FindPropertyRelative("bubblePrefab"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("projectileForce"));

        //r.y += lineHeight + 2;
        //EditorGUI.PropertyField(r, property.FindPropertyRelative("projectileLifetime"));

        //r.y += lineHeight + 2;
        //EditorGUI.PropertyField(r, property.FindPropertyRelative("spreadInDegrees"));

        //r.y += lineHeight + 2;
        //EditorGUI.PropertyField(r, property.FindPropertyRelative("attackAnimationTrigger"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
#endif