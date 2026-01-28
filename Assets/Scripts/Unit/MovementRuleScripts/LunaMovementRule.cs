using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LunaMovementRule", menuName = "MovementRuleSO/LunaMovementRule")]
public class LunaMovementRule : TileCheckRule
{
    public LunaMovementRule()
    {
        teleportTypeMovement = false;
        mov = 2;
    }
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to,List<Node> path = null)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        
        return (dx + dy <= 2)&&(dx + dy != 0);
    }
}

