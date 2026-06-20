using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Condition
{
    public abstract bool Check(Enemy enemy);
}
