using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoshuAttackRule", menuName = "AttackRuleSO/RoshuAttackRule")]
public class RoshuAttackRule : TileCheckRule
{
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to,List<Node> path = null)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        return ((dx == 1 && dy == 0) || (dx == 0 && dy == 1));
    }
}
