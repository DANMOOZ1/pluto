using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardMovementRule", menuName = "MovementRuleSO/StandardMovementRule")]
public class StandardMovementRule : MovementRule
{
    public override bool MovementRuleFunc(List<Node> path, int mov)
    {
        if (path.Count > mov) return false;
        return true;
    }
}

