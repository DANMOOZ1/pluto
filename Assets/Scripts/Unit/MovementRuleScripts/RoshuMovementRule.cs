using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoshuMovementRule", menuName = "MovementRuleSO/RoshuMovementRule")]
public class RoshuMovementRule : TileCheckRule
{
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        int dz = to.z - from.z;

        if (dz != 0)
        {
            if(dz == 1 && TileMapManager.Instance.dataOnTiles[to].escalator) return dx == 1 && dy == 1;
            else if(dz == -1 && TileMapManager.Instance.dataOnTiles[from].escalator) return dx == 1 && dy == 1;
            return false;
        }
        return dx == 1 && dy == 1;
    }
}

