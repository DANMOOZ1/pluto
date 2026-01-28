using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LunaAttackRule", menuName = "AttackRuleSO/LunaAttackRule")]
public class LunaAttackRule : TileCheckRule
{
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to, List<Node> path = null)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        return ((dx is 1 or 2 && dy == 0) || (dx == 0 && dy is 1 or 2));
    }
}
