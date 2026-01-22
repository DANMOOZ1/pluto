using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LunaHeatAreaRule", menuName = "HeatAreaRuleSO/LunaHeatAreaRule")]
public class LunaHeatAreaRule : HeatAreaRule
{
    public override List<Vector3Int> HeatAreaRuleFunc(Vector3Int from, Vector3Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (dx > dy)
        {
            Vector3Int v = new Vector3Int(1, 0, 0);
            return new List<Vector3Int>() { to, to + v*(dx/(to.x - from.x)) };
        }else if (dy > dx)
        {
            Vector3Int v = new Vector3Int(0, 1, 0);
            return new List<Vector3Int>() { to, to + v*(dy/(to.y - from.y)) };
        }
        else
        {
            Debug.LogWarning("HeatArea 계산 오류 발생");
            return new List<Vector3Int>() { to };
        }
    }
}

