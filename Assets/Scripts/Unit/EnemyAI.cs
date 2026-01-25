using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 7f;
    
    private Unit mySelf;
    public Unit target;
    public bool canAttack;
    
    private void Awake()
    {
        mySelf = GetComponent<Unit>();
    }

    public void Move()
    {
        print(mySelf.unitName+"------------------------------");
        
        canAttack = false;
        target = FindNearestTarget();
        
        if (target == null)
        {
            TransitionToNextState();
            return;
        }

        Debug.Log("타겟 찾음: " + target.name);
        
        if (TryAttackFromCurrentPosition())
            return;
        
        if (TryMoveAndAttack())
            return;

        if (TryMove())
            return;
        
        
        Debug.Log("이동 불가");
        TransitionToNextState();
    }

    private bool TryAttackFromCurrentPosition()
    {
        if (CanAttackFrom(mySelf.cellPosition, target.cellPosition))
        {
            canAttack = true;
            GameManager.Instance.UpdateBattleState(BattleState.Combat);
            return true;
        }
        return false;
    }

    private bool TryMoveAndAttack()
    {
        //적 위치를 공격할 수 있는 모든 타일 라스트(도달 가능한)
        List<Vector3Int> attackableTiles = FindAttackableTilesAfterMove(target.cellPosition);
        
        //적을 공격할 수 있는 타일이 도달 가능한 경우
        if (attackableTiles.Count > 0)
        {
            Vector3Int bestTile = FindBestAttackPosition(attackableTiles);

            List<Node> path =
                TileMapManager.Instance.GeneratePathTo(mySelf.cellPosition, bestTile, mySelf.movementRule);
            //이동 후 바로 공격이 가능한 경우
            if(path.Count == 1) canAttack = true;
            //아닌 경우엔 bestTile로 가는 path를 따라감
            else bestTile = new Vector3Int(path[0].x, path[0].y, path[0].z);
            
            mySelf.StartMoving(bestTile);
            return true;
        }
        return false;
    }

    public Unit FindNearestTarget()
    {
        List<Unit> enemyUnits = UnitManager.Instance.allyUnits;
        if (enemyUnits == null || enemyUnits.Count == 0)
            return null;

        Unit nearestTarget = null;
        float minDistance = float.MaxValue;
        Vector3Int myPos = mySelf.cellPosition;

        foreach (Unit enemy in enemyUnits)
        {
            float distance = Vector3Int.Distance(enemy.cellPosition, myPos);
            if (distance < minDistance)
            {
                nearestTarget = enemy;
                minDistance = distance;
            }
        }

        return minDistance <= detectionRange ? nearestTarget : null;
    }

    private List<Vector3Int> FindAttackableTilesAfterMove(Vector3Int targetPos)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        Vector3Int selfPos = mySelf.cellPosition;
        
        // 적을 공격 가능한 타일을 찾기
        List<Vector3Int> atkTiles = new List<Vector3Int>();
        foreach (Vector3Int tilePos in TileMapManager.Instance.cellPosGraph.Keys)
        {
            if (mySelf.atkRule.TileCheckRuleFunc(tilePos, targetPos)) atkTiles.Add(tilePos);
        }

        foreach (Vector3Int atkPos in atkTiles)
        {
            if(TileMapManager.Instance.GeneratePathTo(selfPos,atkPos,mySelf.movementRule) != null) result.Add(atkPos);
        }

        return result;
    }

    private Vector3Int FindBestAttackPosition(List<Vector3Int> candidates)
    {
        float minDistance = float.MaxValue;
        Vector3Int myPos = mySelf.cellPosition;
        Vector3Int bestPos = new Vector3Int();
        
        foreach (Vector3Int v in candidates)
        {
            float distance = Vector3Int.Distance(v, myPos);
            if (distance < minDistance)
            {
                bestPos = v;
                minDistance = distance;
            }
        }

        return bestPos;
    }

    
    private bool TryMove()
    {
        Vector3Int myPos = mySelf.cellPosition;
        List<Vector3Int> movementTiles = new List<Vector3Int>();
        foreach (Vector3Int tilePos in TileMapManager.Instance.cellPosGraph.Keys)
        {
            if(mySelf.movementRule.TileCheckRuleFunc(myPos, tilePos)) movementTiles.Add(tilePos);
        }

        List<Vector3Int> moveableTiles = new List<Vector3Int>();
        foreach (Vector3Int tilePos in movementTiles)
        {
            if(TileMapManager.Instance.GeneratePathTo(myPos,tilePos,mySelf.movementRule) != null) moveableTiles.Add(tilePos);
        }

        int minDistance = int.MaxValue;
        Vector3Int? bestPos = null;
        Vector3Int targetPos = target.cellPosition;
        
        foreach (Vector3Int movePos in moveableTiles)
        {
            List<Node> path = TileMapManager.Instance.GeneratePathTo(movePos, targetPos, mySelf.movementRule, true);
            if (path == null) continue;

            int distance = path.Count;
            if (distance < minDistance)
            {
                bestPos = movePos;
                minDistance = distance;
            }
        }

        if (bestPos.HasValue)
        {
            mySelf.StartMoving(bestPos.Value);
            print(bestPos.Value+"ㄱㄱ");
            foreach (Node n in TileMapManager.Instance.GeneratePathTo(bestPos.Value, targetPos, mySelf.movementRule, true))
            {
                print(n.x + "," + n.y + "," + n.z);
            }
            return true;
        }
        
        return false;
    }
    private bool CanAttackFrom(Vector3Int fromPos, Vector3Int targetPos)
    {
        return mySelf.atkRule.TileCheckRuleFunc(fromPos, targetPos);
    }

    private void TransitionToNextState()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Next);
    }
}