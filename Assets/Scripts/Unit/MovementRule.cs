using System.Collections.Generic;
using UnityEngine;

public abstract class MovementRule : ScriptableObject
{
    public abstract bool MovementRuleFunc(List<Node> path, Vector3Int pos, int mov);
}
