using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolCondition", menuName = "ScriptableObjects/Conditions/Bool", order = 1)]
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
