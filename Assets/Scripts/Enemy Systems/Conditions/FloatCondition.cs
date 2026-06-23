using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class FloatCondition : Condition
{
    public enum Operator
    {
        Equal,
        GreaterThan,
        LessThan,
        Between,
        NotBetween
    }

    public string attributeName;
    public Operator operatorType;
    public float value = 0f;
    public float value2 = 0f;   // Used for "Between" operator

    public override bool Check(Enemy enemy)
    {
        if (!enemy.GetData().HasAttribute(attributeName))
            return false;

        float data = enemy.GetData().GetAttribute<float>(attributeName);

        switch (operatorType)
        {
            case Operator.Equal:
                return data > value - 0.0001 && data < value + 0.0001;
            case Operator.GreaterThan:
                return data > value;
            case Operator.LessThan:
                return data < value;
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
[CustomPropertyDrawer(typeof(FloatCondition))]
public class FloatConditionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Float Condition", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("attributeName"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("operatorType"));

        FloatCondition.Operator opSelected = (FloatCondition.Operator)property.FindPropertyRelative("operatorType").enumValueIndex;

        if (opSelected == FloatCondition.Operator.Between || opSelected == FloatCondition.Operator.NotBetween)
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value"), new GUIContent("Min"));

            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value2"), new GUIContent("Max"));

            Rect messageRect = position;
            messageRect.x += 20;
            messageRect.y += 110;
            messageRect.height = 40;
            EditorGUI.HelpBox(messageRect, "Between is inclusive. Keep that in mind when setting your ranges.", MessageType.Info);
        }
        else
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("value"));
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        FloatCondition.Operator opSelected = (FloatCondition.Operator)property.FindPropertyRelative("operatorType").enumValueIndex;
        if (opSelected == FloatCondition.Operator.Between || opSelected == FloatCondition.Operator.NotBetween)
            return EditorGUIUtility.singleLineHeight * 8 + 10;
        else
            return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif
