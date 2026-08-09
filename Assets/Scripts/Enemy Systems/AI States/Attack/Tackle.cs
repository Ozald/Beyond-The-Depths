using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Tackle : AIState
{
    public float tackleSpeed = 5f;

    public override void OnEnter(Enemy enemy)
    {
        
    }

    public override void OnExit(Enemy enemy)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.left * tackleSpeed, ForceMode2D.Impulse);
        }
    }

    public override void OnFixedUpdate(Enemy enemy)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = Vector2.right;
            rb.velocity = direction * tackleSpeed * 5;
        }
    }

    public override void OnUpdate(Enemy enemy)
    {
        
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Tackle))]
public class TackleEditor : PropertyDrawer
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
        EditorGUI.PropertyField(r, property.FindPropertyRelative("tackleSpeed"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
#endif
