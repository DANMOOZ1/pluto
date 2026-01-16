using System.Collections.Generic;
using UnityEngine;

public abstract class AttackRule : ScriptableObject
{
    public abstract List<Vector3Int> AttackRuleFunc(Vector3Int pos, IEnumerable<Vector3Int> targets);
}
