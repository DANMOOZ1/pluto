using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunaMovementRule", menuName = "MovementRuleSO/RunaMovementRule")]
public class RunaMovementRule : MovementRule
{
    public override bool MovementRuleFunc(List<Node> path, Vector3Int pos, int mov)
    {
        if (path.Count <= 1) return true;
        return false;
    }
}

