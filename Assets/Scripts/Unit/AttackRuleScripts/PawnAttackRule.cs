using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PawnAttackRule", menuName = "AttackRuleSO/PawnAttackRule")]
public class PawnAttackRule : TileCheckRule
{
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to,List<Node> path = null)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        
        
        return (dx <= 1 && dy <= 1)&&!(dx == 0 && dy == 0);
    }
}
