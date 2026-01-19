using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public void Move()
    {
        Unit target = FindTarget();

        if (target != null)
        {
            
        }
    }

    private Unit FindTarget()
    {
        List<Unit> EnemyUnits = UnitManager.Instance.accessibleUnits;
        Unit mySelf = gameObject.GetComponent<Unit>();
        
        //가장 가까운 적 찾기
        Vector3Int pos = mySelf.cellPosition;
        float minDist = float.MaxValue;
        Unit target = null;
        foreach(Unit u in EnemyUnits)
        {
            float dist = Vector3Int.Distance(u.cellPosition, pos);
            if (dist < minDist)
            {
                target = u;
                minDist = dist;
            }
        }
        // AI의 인식 범위는 7칸
        if (minDist > 7) return null;
        
        return target;
    }
}
