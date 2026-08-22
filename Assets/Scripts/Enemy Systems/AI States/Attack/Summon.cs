using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Summon : AIState
{
    public GameObject enemyToSummon;
    public int numberOfEnemies = 3;

    public override void OnEnter(Enemy enemy)
    {
        enemy.StartCoroutine(SummonEnemies(enemy));
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

    private IEnumerator SummonEnemies(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject.Instantiate(enemyToSummon, enemy.transform.position, Quaternion.identity);
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Summon))]
public class SummonEditor : PropertyDrawer
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
        EditorGUI.PropertyField(r, property.FindPropertyRelative("enemyToSummon"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("numberOfEnemies"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
#endif