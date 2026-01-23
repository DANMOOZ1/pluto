using UnityEngine;

[CreateAssetMenu(fileName = "PawnMovementRule", menuName = "MovementRuleSO/PawnMovementRule")]
public class PawnMovementRule : TileCheckRule
{
    public override bool TileCheckRuleFunc(Vector3Int from, Vector3Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        return (dx <= 1 && dy <= 1)&&(dx != 0 && dy != 0);
    }
}
