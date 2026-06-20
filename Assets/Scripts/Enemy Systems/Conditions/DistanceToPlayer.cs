using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/Transition Conditions/Distance To Player")]
public class DistanceToPlayer : Condition
{
    public enum ComparisonOperator
    {
        LessThanOrEqual,
        GreaterThanOrEqual,
        Between,
        NotBetween
    }

    public float distanceThreshold = 20f;
    public float distanceThreshold2 = 20f;
    public ComparisonOperator comparisonOperator = ComparisonOperator.LessThanOrEqual;

    public override bool Check(Enemy enemy)
    {
        Transform player = PlayerManager.instance.transform;
        float distance = Vector3.Distance(player.position, enemy.transform.position);

        switch (comparisonOperator)
        {
            case ComparisonOperator.LessThanOrEqual:
                return distance <= distanceThreshold;
            case ComparisonOperator.GreaterThanOrEqual:
                return distance >= distanceThreshold;
            case ComparisonOperator.Between:
                return distance >= distanceThreshold && distance <= distanceThreshold2;
            case ComparisonOperator.NotBetween:
                return distance < distanceThreshold && distance > distanceThreshold2;
            default:
                Debug.LogError("Unsupported operator type");
                return false;
        }
    }
}

/*****************************************************************************/

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DistanceToPlayer))]
public class DistanceToPlayerEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Player Distance Condition", titleStyle);

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("comparisonOperator"));

        DistanceToPlayer.ComparisonOperator opSelected = (DistanceToPlayer.ComparisonOperator)property.FindPropertyRelative("comparisonOperator").enumValueIndex;

        if (opSelected == DistanceToPlayer.ComparisonOperator.Between || opSelected == DistanceToPlayer.ComparisonOperator.NotBetween)
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("distanceThreshold"), new GUIContent("Min"));

            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("distanceThreshold2"), new GUIContent("Max"));

            Rect messageRect = position;
            messageRect.y += 110;
            messageRect.height = 40;
            EditorGUI.HelpBox(messageRect, "Between is inclusive. Keep that in mind when setting your ranges.", MessageType.Info);
        }
        else
        {
            r.y += lineHeight + 2;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("distanceThreshold"), new GUIContent("Value"));
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        DistanceToPlayer.ComparisonOperator opSelected = (DistanceToPlayer.ComparisonOperator)property.FindPropertyRelative("comparisonOperator").enumValueIndex;
        if (opSelected == DistanceToPlayer.ComparisonOperator.Between || opSelected == DistanceToPlayer.ComparisonOperator.NotBetween)
            return EditorGUIUtility.singleLineHeight * 8 + 10;
        else
            return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif
