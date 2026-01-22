using System.Collections.Generic;
using UnityEngine;

public abstract class HeatAreaRule : ScriptableObject
{
    public abstract List<Vector3Int> HeatAreaRuleFunc(Vector3Int from, Vector3Int to);
}