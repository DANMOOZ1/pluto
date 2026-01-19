using System.Collections.Generic;
using UnityEngine;

public abstract class TileCheckRule : ScriptableObject
{
    public abstract bool TileCheckRuleFunc(Vector3Int from, Vector3Int to);
}
