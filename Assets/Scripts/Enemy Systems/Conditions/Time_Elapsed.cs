using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Time_Elapsed : Condition
{
    public float timeElapsed = 5f;

    public override bool Check(Enemy enemy)
    {
        if (enemy.stateTimer > timeElapsed)
            return true;

        return false;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Time_Elapsed))]
public class TimerConditionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Time Elapsed", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("timeElapsed"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif