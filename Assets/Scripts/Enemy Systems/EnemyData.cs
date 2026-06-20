using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum TransitionMode : byte
{
    FirstValid,         // The first valid transition found will be picked
    Random,             // A random valid transition would be picked disregarding its probability weights
    WeightedRandom      // A random valid transition would be picked based on its probability weights
}

[Serializable]
public class Transition
{
    public int weight;
    public AIState toState;

    [SerializeReference]
    public Condition condition;
}

[Serializable]
public class StateNode
{
    public TransitionMode transitionMode;
    public AIState state;
    public List<Transition> transitions;
}

[CreateAssetMenu(menuName = "Enemy AI/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int HP;
    public float invincibilityCooldown;

    public AIState initialState;
    public List<StateNode> states;
}

/************************************************************************/

// THIS CODE IS DEMONICALLY COMPLICATED AND I HATE IT BUT IT WORKS SO HERE WE ARE

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    private static Type[] conditionTypes;

    // Find all subclasses of Condition across the project file
    static EnemyDataEditor()
    {
        conditionTypes = TypeCache
            .GetTypesDerivedFrom<Condition>()
            .Where(t => !t.IsAbstract)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "states", "initialState");
        EditorGUILayout.Space(15);

        Rect rect = EditorGUILayout.GetControlRect(false, 2); // 1 pixel high
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f)); // Soft gray line
        EditorGUILayout.Space(6);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fixedHeight = 30;
        headerStyle.fontSize = 30;
        headerStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.LabelField("State Machine", headerStyle);
        EditorGUILayout.Space(25);

        DrawAllStates();
        EditorGUILayout.Space(12);

        rect = EditorGUILayout.GetControlRect(false, 2); // 1 pixel high
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f)); // Soft gray line

        serializedObject.ApplyModifiedProperties();
    }

    /******************************** STATE MACHINE DRAW CALLS *******************************/

    public void DrawAllStates()
    {
        SerializedProperty states = serializedObject.FindProperty("states");
        SerializedProperty initialState = serializedObject.FindProperty("initialState");

        EditorGUILayout.PropertyField(initialState);
        EditorGUILayout.Space();

        for (int i = 0; i < states.arraySize; i++)
        {
            SerializedProperty stateNode = states.GetArrayElementAtIndex(i);
            DrawState(stateNode);
        }

        if (GUILayout.Button("Add State"))
        {
            AddState();
        }

        if (GUILayout.Button("Clear All States"))
        {
            states.ClearArray();

            Undo.RecordObject(target, "Clear All States");
            EditorUtility.SetDirty(target);
        }
    }

    public void DrawState(SerializedProperty stateNode)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        string label = stateNode.FindPropertyRelative("state").objectReferenceValue != null
            ? stateNode.FindPropertyRelative("state").objectReferenceValue.name
            : "Empty State";

        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;
        foldoutStyle.fontSize = 14;
        foldoutStyle.fixedHeight = 14;
        foldoutStyle.margin = new RectOffset(10, 0, 5, 5);

        EditorGUILayout.BeginHorizontal();
        stateNode.isExpanded = EditorGUILayout.Foldout(stateNode.isExpanded, label, true, foldoutStyle);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete State"))
        {
            stateNode.DeleteCommand();
            Undo.RecordObject(target, "Delete State");
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.EndHorizontal();

        if (stateNode.isExpanded)
        {
            EditorGUILayout.Space(30);
            EditorGUI.indentLevel++;

            SerializedProperty state = stateNode.FindPropertyRelative("state");
            EditorGUILayout.PropertyField(state);

            SerializedProperty transitionMode = stateNode.FindPropertyRelative("transitionMode");
            EditorGUILayout.PropertyField(transitionMode);

            EditorGUILayout.Space(15);

            GUIStyle subHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            subHeaderStyle.fontSize = 20;
            subHeaderStyle.fixedHeight = 24;
            subHeaderStyle.alignment = TextAnchor.MiddleCenter;

            Rect rect = EditorGUILayout.GetControlRect(false, 2); // 1 pixel high
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f)); // Soft gray line
            EditorGUILayout.Space(6);

            EditorGUILayout.LabelField("Transitions", subHeaderStyle);
            EditorGUILayout.Space(10);

            DrawTransitions(stateNode);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    public void DrawTransitions(SerializedProperty stateNode)
    {
        SerializedProperty transitions = stateNode.FindPropertyRelative("transitions");
        SerializedProperty transitionMode = stateNode.FindPropertyRelative("transitionMode");

        for (int j = 0; j < transitions.arraySize; j++)
        {
            DrawTransition(transitions.GetArrayElementAtIndex(j), transitionMode);
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Transition", GUILayout.Width(150)))
        {
            AddTransition(transitions);
        }
        EditorGUILayout.EndHorizontal();
    }

    public void DrawTransition(SerializedProperty transition, SerializedProperty transitionMode)
    {
        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
        style.margin = new RectOffset(10, 10, 0, 0);
        EditorGUILayout.BeginVertical(style);

        string label = transition.FindPropertyRelative("toState").objectReferenceValue != null
            ? transition.FindPropertyRelative("toState").objectReferenceValue.name
            : "Empty Transition";

        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;
        foldoutStyle.fontSize = 14;
        foldoutStyle.fixedHeight = 14;
        foldoutStyle.margin = new RectOffset(10, 0, 5, 5);

        transition.isExpanded = EditorGUILayout.Foldout(transition.isExpanded, label, true, foldoutStyle);

        if (transition.isExpanded) 
        {
            TransitionMode selected = (TransitionMode)transitionMode.enumValueIndex;

            EditorGUILayout.Space();
            if (selected == TransitionMode.WeightedRandom)
            {
                SerializedProperty weight = transition.FindPropertyRelative("weight");
                EditorGUILayout.PropertyField(weight);
            }

            SerializedProperty toState = transition.FindPropertyRelative("toState");
            EditorGUILayout.PropertyField(toState, label: new GUIContent("Destination"));

            DrawCondition(transition);


            EditorGUILayout.Space();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            transition.DeleteCommand();

            Undo.RecordObject(target, "Delete Transition");
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    public void DrawCondition(SerializedProperty transition)
    {
        SerializedProperty condition = transition.FindPropertyRelative("condition");

        if (condition.managedReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Condition", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Condition"))
            {
                AddCondition(condition);
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Condition", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Condition"))
            {
                condition.managedReferenceValue = null;
                Undo.RecordObject(target, "Delete Condition");
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(condition, true);

            if (condition.managedReferenceValue is IntCondition || condition.managedReferenceValue is BoolCondition || condition.managedReferenceValue is FloatCondition)
                EditorGUILayout.HelpBox("Ensure that there are no typos in the attribute names. It is case-sensitive and must match exactly to an attribute that is stored within the enemy's EnemyContext.", MessageType.Warning);

            
        }

        EditorGUILayout.Space();
    }

    /******************************** CONTEXT PROVIDER DRAW CALLS *****************************/



    /********************************** BUTTON FUNCTIONALITY *******************************/

    private void AddState()
    {
        serializedObject.Update();

        SerializedProperty stateNodes = serializedObject.FindProperty("states");
        int newIndex = stateNodes.arraySize;
        stateNodes.arraySize = newIndex + 1;

        SerializedProperty newStateNode = stateNodes.GetArrayElementAtIndex(newIndex);

        newStateNode.FindPropertyRelative("state").objectReferenceValue = null;
        newStateNode.FindPropertyRelative("transitionMode").enumValueIndex = 0;
        SerializedProperty transitions = newStateNode.FindPropertyRelative("transitions");
        transitions.arraySize = 0;

        newStateNode.isExpanded = true;

        serializedObject.ApplyModifiedProperties();

        Undo.RecordObject(target, "Add State");
        EditorUtility.SetDirty(target);
    }

    private void AddTransition(SerializedProperty transitions)
    {
        serializedObject.Update();

        int newIndex = transitions.arraySize;
        transitions.arraySize++;

        SerializedProperty newTransition = transitions.GetArrayElementAtIndex(newIndex);

        newTransition.FindPropertyRelative("weight").intValue = 1;
        newTransition.FindPropertyRelative("toState").objectReferenceValue = null;
        newTransition.FindPropertyRelative("condition").managedReferenceValue = null;

        newTransition.isExpanded = true;

        serializedObject.ApplyModifiedProperties();

        Undo.RecordObject(target, "Add Transition");
        EditorUtility.SetDirty(target);
    }

    private void AddCondition(SerializedProperty condition)
    {
        GenericMenu menu = new GenericMenu();

        foreach (Type type in conditionTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                serializedObject.Update();

                condition.managedReferenceValue = Activator.CreateInstance(type);

                Undo.RecordObject(target, "Add Condition");
                EditorUtility.SetDirty(target);

                serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }
}

#endif
