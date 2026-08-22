using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Tackle : AIState
{
    public float tackleSpeed = 5f;
    public string tackleAnimationTrigger = "Tackle";

    public override void OnEnter(Enemy enemy)
    {
        enemy.StartCoroutine(BeginTackle(enemy));
    }

    public override void OnExit(Enemy enemy)
    {
    }

    public override void OnFixedUpdate(Enemy enemy)
    {
        
    }

    public override void OnUpdate(Enemy enemy)
    {
        
    }

    #region Helper Methods

    private IEnumerator BeginTackle(Enemy enemy)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (rb != null)
        {
            yield return new WaitForSeconds(0.5f);
            Vector2 direction = (player.position - enemy.transform.position).normalized;

            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(tackleAnimationTrigger);
            }

            yield return new WaitForSeconds(0.25f);
            rb.AddForce(direction * tackleSpeed * 5, ForceMode2D.Impulse);
        }
    }

    #endregion
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

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("tackleAnimationTrigger"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
#endif
