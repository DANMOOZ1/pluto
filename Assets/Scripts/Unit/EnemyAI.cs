using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Unit mySelf;
    public List<Vector3Int> attackableTiles = null;
    public Unit target = null;
    
    //수 많은 버그의 향현, 버그의 집합체, 정신 나갈거 같아ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
    public void Move()
    {
        target = FindTarget();

        if (target != null)
        {
            print("타겟 찾음");
            attackableTiles = AttackableTiles(target.cellPosition);
            
            //현재 상태에서 이동 후 공격가능하지 않을 경우
            if (attackableTiles.Count == 0)
            {
                List<Vector3Int> attackableTilesNearbyTarget = NoMoveAttackTiles(target.cellPosition);
                
                //적 주변에서 적을 공격할 수 있는 타일이 존재하는 경우
                if (attackableTilesNearbyTarget.Count != 0)
                {
                    List<Node> path = null;
                    foreach (Vector3Int tileNearbyTarget in attackableTilesNearbyTarget)
                    {
                        path = TileMapManager.Instance.GeneratePathTo(mySelf.cellPosition,tileNearbyTarget, mySelf.movementRule);
                    }
                    if(path != null) mySelf.StartMoving(new Vector3Int(path[0].x, path[0].y,path[0].z));
                    // 적을 아예 공격할 수 없는 경우
                    else
                    {
                        print("적 공격 불가");
                        GameManager.Instance.UpdateBattleState(BattleState.Next);
                    }
                }
                else
                {
                    // 적을 아예 공격할 수 없는 경우
                    print("적 공격 불가");
                    GameManager.Instance.UpdateBattleState(BattleState.Next);
                }
                // 타겟을 null 로 만들어 move 후 attack 방지
                target = null;
            }
            //현재 위치에서 공격이 가능한 경우
            else if (attackableTiles[0] == mySelf.cellPosition)
            {
                GameManager.Instance.UpdateBattleState(BattleState.Combat);
            }
            //현재 상태에서 이동 후 공격이 가능한 경우
            else
            {
                mySelf.StartMoving(new Vector3Int(attackableTiles[0].x, attackableTiles[0].y, attackableTiles[0].z));
            }
        }
        else GameManager.Instance.UpdateBattleState(BattleState.Next);
    }

    private Unit FindTarget()
    {
        List<Unit> EnemyUnits = UnitManager.Instance.accessibleUnits;
        mySelf = gameObject.GetComponent<Unit>();
        
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

    private List<Vector3Int> AttackableTiles(Vector3Int targetPos)
    {
        List<Vector3Int> dist = new List<Vector3Int>();
        Vector3Int pos = mySelf.cellPosition;
        
        //현재 위치에서 공격이 가능한 경우 검사
        if (mySelf.atkRule.TileCheckRuleFunc(pos, targetPos))
        {
            // 가능하다면 return
            dist.Add(pos);
            return dist;
        }
        
        //이동 후 공격이 가능한 경우 검사
        List<Vector3Int> allTiles = TileMapManager.Instance.dataOnTiles.Keys.ToList();
        allTiles.Remove(targetPos);
        
        foreach (Vector3Int v in allTiles)
        {
            if (!mySelf.movementRule.TileCheckRuleFunc(pos, v)) continue;
            if (mySelf.atkRule.TileCheckRuleFunc(v, targetPos)) dist.Add(v);
        }
        
        return dist;
    }

    private List<Vector3Int> NoMoveAttackTiles(Vector3Int targetPos)
    {
        List<Vector3Int> dist = new List<Vector3Int>();
        List<Vector3Int> allTiles = TileMapManager.Instance.dataOnTiles.Keys.ToList();
        allTiles.Remove(targetPos);

        foreach (Vector3Int v in allTiles)
        {
            if(mySelf.atkRule.TileCheckRuleFunc(v, targetPos))  dist.Add(v);
        }
        
        return dist;
    }
}
