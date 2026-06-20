using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatCondition", menuName = "ScriptableObjects/Conditions/Float", order = 1)]
public class FloatCondition : Condition
{
    public enum Operator
    {
        GreaterThan,
        LessThan,
        Equal,
        Between
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
                return data > value - 0.001 && data < value + 0.001;
            case Operator.GreaterThan:
                return data > value;
            case Operator.LessThan:
                return data < value;
            case Operator.Between:
                return data >= value && data <= value2;
            default:
                Debug.LogError("Unsupported operator type");
                return false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FloatCondition))]
public class FloatConditionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("attributeName"));

        SerializedProperty op =
            serializedObject.FindProperty("operatorType");

        EditorGUILayout.PropertyField(op);

        IntCondition.Operator selected =
            (IntCondition.Operator)op.enumValueIndex;

        if (selected == IntCondition.Operator.Between)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            SerializedProperty min = serializedObject.FindProperty("value");
            SerializedProperty max = serializedObject.FindProperty("value2");

            GUILayout.Label("Min", GUILayout.Width(30));
            min.intValue = EditorGUILayout.IntField(min.intValue, GUILayout.Width(60));

            GUILayout.Label("Max", GUILayout.Width(30));
            max.intValue = EditorGUILayout.IntField(max.intValue, GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Between is inclusive. Keep that in mind when setting your ranges.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
