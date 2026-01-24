using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LunaHeatAreaRule", menuName = "HeatAreaRuleSO/LunaHeatAreaRule")]
public class LunaHeatAreaRule : HeatAreaRule
{
    public override List<Vector3Int> HeatAreaRuleFunc(Vector3Int from, Vector3Int to)
    {
        int dx = to.x - from.x;
        int dy = to.y - from.y;

        if (dy == 0)
        {
            if(dx == 1) return new List<Vector3Int>() { to, to + new Vector3Int((dx/Mathf.Abs(dx)), 0, 0)};
            return new List<Vector3Int>() { to, to - new Vector3Int((dx/Mathf.Abs(dx)), 0, 0)};
        }

        if (dx == 0)
        {
            if(dy == 1) return new List<Vector3Int>() { to, to + new Vector3Int(0, (dy/Mathf.Abs(dy)) , 0)};
            return new List<Vector3Int>() { to, to - new Vector3Int(0, (dy/Mathf.Abs(dy)) , 0)};
        }
        
        Debug.LogWarning("HeatArea 계산 오류 발생");
        return new List<Vector3Int>() { to };
        
    }
}

