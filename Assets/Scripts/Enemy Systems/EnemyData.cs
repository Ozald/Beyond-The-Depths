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

    [SerializeReference]
    public AIState toState;

    [SerializeReference]
    public List<Condition> conditions;
}

[Serializable]
public class StateNode
{
    public TransitionMode transitionMode;

    [SerializeReference]
    public AIState state;

    public List<Transition> transitions;
}

[CreateAssetMenu(menuName = "Enemy AI/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float HP;
    public float invincibilityCooldown;

    [SerializeReference]
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
    private static Type[] stateTypes;

    // Find all subclasses of Condition across the project file
    static EnemyDataEditor()
    {
        conditionTypes = TypeCache
            .GetTypesDerivedFrom<Condition>()
            .Where(t => !t.IsAbstract)
            .ToArray();

        stateTypes = TypeCache
            .GetTypesDerivedFrom<AIState>()
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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Initial State");

        string dropdownLabel = initialState.managedReferenceValue != null ? initialState.managedReferenceValue.GetType().Name : "None";
        if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Keyboard))
        {
            PickExistingState(initialState);
        }
        EditorGUILayout.EndHorizontal();


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

        SerializedProperty state = stateNode.FindPropertyRelative("state");

        string label = state.managedReferenceValue != null
            ? state.managedReferenceValue.GetType().Name
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

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("State Type");
            string dropdownLabel = state.managedReferenceValue != null ? state.managedReferenceValue.GetType().Name : "None";

            if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Keyboard))
            {
                PickState(state);
            }
            EditorGUILayout.EndHorizontal();

            SerializedProperty transitionMode = stateNode.FindPropertyRelative("transitionMode");
            EditorGUILayout.PropertyField(transitionMode);

            if (state.managedReferenceValue != null)
            {
                EditorGUILayout.Space();
                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.margin = new RectOffset(10, 0, 5, 5);

                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.PropertyField(state);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

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

        SerializedProperty toState = transition.FindPropertyRelative("toState");

        string label = toState.managedReferenceValue != null
            ? toState.managedReferenceValue.GetType().Name
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

            //EditorGUILayout.PropertyField(toState, label: new GUIContent("Destination"));
            string dropdownLabel = toState.managedReferenceValue != null ? toState.managedReferenceValue.GetType().Name : "None";

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Destination");
            if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Keyboard))
            {
                PickExistingState(toState);
            }
            EditorGUILayout.EndHorizontal();

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
        SerializedProperty conditions = transition.FindPropertyRelative("conditions");

        if (conditions.arraySize > 0)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(20);
            for (int i = 0; i < conditions.arraySize; i++)
            {
                SerializedProperty condition = conditions.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(condition, true);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    conditions.DeleteArrayElementAtIndex(i);
                    Undo.RecordObject(target, "Delete Condition");
                    EditorUtility.SetDirty(target);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Condition", GUILayout.Width(100)))
            {
                AddCondition(conditions);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Condition", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Condition"))
            {
                AddCondition(conditions);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        //if (condition.managedReferenceValue == null)
        //{
        //    EditorGUILayout.BeginHorizontal();

        //    EditorGUILayout.LabelField("Condition", GUILayout.Width(100));
        //    GUILayout.FlexibleSpace();
        //    if (GUILayout.Button("Add Condition"))
        //    {
        //        AddCondition(condition);
        //    }

        //    EditorGUILayout.EndHorizontal();
        //}
        //else
        //{
        //    EditorGUILayout.BeginHorizontal();

        //    EditorGUILayout.LabelField("Condition", GUILayout.Width(100));
        //    GUILayout.FlexibleSpace();
        //    if (GUILayout.Button("Delete Condition"))
        //    {
        //        condition.managedReferenceValue = null;
        //        Undo.RecordObject(target, "Delete Condition");
        //        EditorUtility.SetDirty(target);
        //    }

        //    EditorGUILayout.EndHorizontal();

        //    EditorGUILayout.Space(10);
        //    EditorGUILayout.PropertyField(condition, true);

        //    if (condition.managedReferenceValue is IntCondition || condition.managedReferenceValue is BoolCondition || condition.managedReferenceValue is FloatCondition)
        //        EditorGUILayout.HelpBox("Ensure that there are no typos in the attribute names. It is case-sensitive and must match exactly to an attribute that is stored within the enemy's EnemyContext.", MessageType.Warning);


        //}

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

        SerializedProperty conditions = newTransition.FindPropertyRelative("conditions");
        conditions.arraySize = 0;

        newTransition.isExpanded = true;

        serializedObject.ApplyModifiedProperties();

        Undo.RecordObject(target, "Add Transition");
        EditorUtility.SetDirty(target);
    }

    private void AddCondition(SerializedProperty conditions)
    {
        GenericMenu menu = new GenericMenu();

        foreach (Type type in conditionTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                serializedObject.Update();

                int newIndex = conditions.arraySize;
                conditions.arraySize++;
                SerializedProperty newCondition = conditions.GetArrayElementAtIndex(newIndex);
                newCondition.managedReferenceValue = Activator.CreateInstance(type);

                Undo.RecordObject(target, "Add Condition");
                EditorUtility.SetDirty(target);

                serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private void PickState(SerializedProperty stateNode)
    {
        GenericMenu menu = new GenericMenu();

        foreach (Type type in stateTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                serializedObject.Update();

                stateNode.managedReferenceValue = Activator.CreateInstance(type);

                Undo.RecordObject(target, "Add State to State Node");
                EditorUtility.SetDirty(target);

                serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private void PickExistingState(SerializedProperty destination)
    {
        GenericMenu menu = new GenericMenu();

        SerializedProperty statesArray = serializedObject.FindProperty("states");

        for (int i = 0; i < statesArray.arraySize; i++)
        {
            SerializedProperty stateNode = statesArray.GetArrayElementAtIndex(i);
            SerializedProperty state = stateNode.FindPropertyRelative("state");

            if (state.managedReferenceValue != null)
            {
                string stateName = state.managedReferenceValue.GetType().Name;
                AIState stateRef = state.managedReferenceValue as AIState;


                menu.AddItem(new GUIContent(stateName), false, () =>
                {
                    serializedObject.Update();

                    destination.managedReferenceValue = stateRef;

                    Undo.RecordObject(target, "Add State to Transition");
                    EditorUtility.SetDirty(target);

                    serializedObject.ApplyModifiedProperties();
                });
            }

            
        }

        menu.ShowAsContext();
    }
}

#endif
