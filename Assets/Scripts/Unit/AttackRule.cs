using System.Collections.Generic;
using UnityEngine;

public abstract class AttackRule : ScriptableObject
{
    public abstract bool AttackRuleFunc(List<Node> path, Vector3Int pos);
}
