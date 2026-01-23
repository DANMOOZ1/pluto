using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 7f;
    
    private Unit mySelf;
    public Unit target;
    private List<Vector3Int> attackableTiles;
    
    private void Awake()
    {
        mySelf = GetComponent<Unit>();
    }

    public void Move()
    {
        target = FindNearestTarget();
        
        if (target == null)
        {
            TransitionToNextState();
            return;
        }

        Debug.Log("타겟 찾음: " + target.name);
        attackableTiles = FindAttackableTilesAfterMove(target.cellPosition);

        if (TryAttackFromCurrentPosition())
            return;

        if (TryMoveAndAttack())
            return;

        Debug.Log("적 공격 불가");
        TransitionToNextState();
    }

    private bool TryAttackFromCurrentPosition()
    {
        if (CanAttackFrom(mySelf.cellPosition, target.cellPosition))
        {
            GameManager.Instance.UpdateBattleState(BattleState.Combat);
            return true;
        }
        return false;
    }

    private bool TryMoveAndAttack()
    {
        if (attackableTiles.Count > 0)
        {
            Vector3Int bestTile = FindBestAttackPosition(attackableTiles);
            mySelf.StartMoving(bestTile);
            return true;
        }

        // 이동 범위 밖에서 공격 가능한 위치 탐색
        Vector3Int reachableAttackTile = FindReachableAttackPosition();
        if (reachableAttackTile != Vector3Int.zero)
        {
            List<Node> path = TileMapManager.Instance.GeneratePathTo(
                mySelf.cellPosition, 
                reachableAttackTile, 
                mySelf.movementRule
            );

            if (path != null && path.Count > 0)
            {
                mySelf.StartMoving(new Vector3Int(path[0].x, path[0].y, path[0].z));
                target = null; // 이동 후 공격 방지
                return true;
            }
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
        Vector3Int myPos = mySelf.cellPosition;

        // 현재 위치에서 공격 가능한지 먼저 확인
        if (CanAttackFrom(myPos, targetPos))
        {
            result.Add(myPos);
            return result;
        }

        // 이동 가능한 타일 중 공격 가능한 위치 찾기
        List<Vector3Int> movableTiles = GetMovableTiles(myPos, targetPos);
        
        foreach (Vector3Int tile in movableTiles)
        {
            if (CanAttackFrom(tile, targetPos))
            {
                result.Add(tile);
            }
        }

        return result;
    }

    private Vector3Int FindReachableAttackPosition()
    {
        List<Vector3Int> allAttackPositions = GetAllAttackPositions(target.cellPosition);
        Vector3Int myPos = mySelf.cellPosition;
        
        // 거리 순으로 정렬하여 가장 가까운 위치부터 시도
        var sortedPositions = allAttackPositions
            .OrderBy(pos => Vector3Int.Distance(pos, myPos))
            .ToList();

        foreach (Vector3Int attackPos in sortedPositions)
        {
            List<Node> path = TileMapManager.Instance.GeneratePathTo(
                myPos, 
                attackPos, 
                mySelf.movementRule
            );

            if (path != null && path.Count > 0)
            {
                return attackPos;
            }
        }

        return Vector3Int.zero;
    }

    private List<Vector3Int> GetMovableTiles(Vector3Int fromPos, Vector3Int excludePos)
    {
        List<Vector3Int> allTiles = TileMapManager.Instance.dataOnTiles.Keys.ToList();
        List<Vector3Int> movableTiles = new List<Vector3Int>();

        foreach (Vector3Int tile in allTiles)
        {
            if (tile == excludePos) continue;
            
            if (mySelf.movementRule.TileCheckRuleFunc(fromPos, tile))
            {
                movableTiles.Add(tile);
            }
        }

        return movableTiles;
    }

    private List<Vector3Int> GetAllAttackPositions(Vector3Int targetPos)
    {
        List<Vector3Int> allTiles = TileMapManager.Instance.dataOnTiles.Keys.ToList();
        List<Vector3Int> attackPositions = new List<Vector3Int>();

        foreach (Vector3Int tile in allTiles)
        {
            if (tile == targetPos) continue;
            
            if (CanAttackFrom(tile, targetPos))
            {
                attackPositions.Add(tile);
            }
        }

        return attackPositions;
    }

    private Vector3Int FindBestAttackPosition(List<Vector3Int> candidates)
    {
        // 현재는 첫 번째 타일 반환, 필요시 우선순위 로직 추가 가능
        // 예: 가장 안전한 위치, 다른 적과의 거리 등 고려
        return candidates[0];
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