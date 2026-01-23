using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PawnHeatAreaRule", menuName = "HeatAreaRuleSO/PawnHeatAreaRule")]
public class PawnHeatAreaRule : HeatAreaRule
{
    public override List<Vector3Int> HeatAreaRuleFunc(Vector3Int from, Vector3Int to)
    {
        return new List<Vector3Int>(){to};
    }
}

