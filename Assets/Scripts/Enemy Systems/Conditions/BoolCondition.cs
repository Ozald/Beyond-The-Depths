using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BoolCondition : Condition
{
    public string variableName;
    public bool value;

    public override bool Check(Enemy enemy)
    {
        if (!enemy.GetData().HasAttribute(variableName))
            return false;

        bool data = enemy.GetData().GetAttribute<bool>(variableName);
        return data == value;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BoolCondition))]
public class BoolConditionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Boolean Condition", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("variableName"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("value"), new GUIContent("Is True"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif