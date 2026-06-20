using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class IntCondition : Condition
{
    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Between,
        NotBetween
    }

    public string attributeName;
    public Operator operatorType;
    public int value = 0;
    public int value2 = 0; // Used for "Between" operator

    public override bool Check(Enemy enemy)
    {
        if (!enemy.GetData().HasAttribute(attributeName))
            return false;

        int data = enemy.GetData().GetAttribute<int>(attributeName);

        switch (operatorType)
        {
            case Operator.Equal:
                return data == value;
            case Operator.NotEqual:
                return data != value;
            case Operator.GreaterThan:
                return data > value;
            case Operator.LessThan:
                return data < value;
            case Operator.GreaterThanOrEqual:
                return data >= value;
            case Operator.LessThanOrEqual:
                return data <= value;
            case Operator.Between:
                return data >= value && data <= value2;
            case Operator.NotBetween:
                return data < value || data > value2;
            default:
                Debug.LogError("Unsupported operator type");
                return false;
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(IntCondition))]
public class IntConditionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Integer Condition", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("attributeName"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("operatorType"));

        IntCondition.Operator opSelected = (IntCondition.Operator)property.FindPropertyRelative("operatorType").enumValueIndex;

        if (opSelected == IntCondition.Operator.Between || opSelected == IntCondition.Operator.NotBetween)
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value"), new GUIContent("Min"));

            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value2"), new GUIContent("Max"));

            Rect messageRect = position;
            messageRect.y += 110;
            messageRect.height = 40;
            EditorGUI.HelpBox(messageRect, "Between is inclusive. Keep that in mind when setting your ranges.", MessageType.Info
);
        }
        else
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value"));
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        IntCondition.Operator opSelected = (IntCondition.Operator)property.FindPropertyRelative("operatorType").enumValueIndex;
        if (opSelected == IntCondition.Operator.Between || opSelected == IntCondition.Operator.NotBetween)
            return EditorGUIUtility.singleLineHeight * 8 + 10;
        else
            return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif
