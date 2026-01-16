using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardAttackRule", menuName = "AttackRuleSO/StandardAttackRule")]
public class StandardAttackRule : AttackRule
{
    public override List<Vector3Int> AttackRuleFunc(Vector3Int pos, IEnumerable<Vector3Int> targets)
    {
        List<Vector3Int> ret = new List<Vector3Int>();
        
        foreach (Vector3Int target in targets)
        {
            if (Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y) + Mathf.Abs(pos.z - target.z) <= 1) ret.Add(target);
        }
        return ret;
    }
}

