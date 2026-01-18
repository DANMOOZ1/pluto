using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoshuMovementRule", menuName = "MovementRuleSO/RoshuMovementRule")]
public class RoshuMovementRule : MovementRule
{
    public override bool MovementRuleFunc(List<Node> path, Vector3Int pos, int mov)
    {
        if (path.Count > 1) return false;
        if (path.Count == 0) return true;
        if(Mathf.Abs(path[0].x - pos[0]) + Mathf.Abs(path[0].y - pos[1]) == 2) return true;
        return false;
    }
}

