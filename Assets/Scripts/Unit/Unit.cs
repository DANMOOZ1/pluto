using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    //유닛 데이터 
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public int def;
    public int spd;
    public int foc;
    public int rng;
    public int hta;
    public Sprite sprite;
    public int mov;
    public int level;
    public bool isAlly;
    public Sprite portrait;
    public MovementRule movementRule;
    public AttackRule atkRule;
    
    //인게임 유닛 테이터
    public Vector3Int cellPosition; // cell 좌표임
    public List<Node> currentPath = null;//이동을 시작할때 목표 타일까지의 경로를 나타냄
    public Dictionary<Vector3Int,List<Node>> accessibleTiles = null;// 현재 시점에서 이동 가능한 타일을 나타냄
    public List<Vector3Int> attackableTiles = new List<Vector3Int>();// 현재 시점에서 공격 가능한 타일을 나타냄
    public float moveSpeed = 5f;  // 이동 속도
    private int currentPathIndex = 0;  // 현재 목표 노드 인덱스
    private bool isMoving = false;  // 이동 중인지 확인

    void Update()
    {
        // 경로 시각화 (디버그용)
        // if(currentPath != null)
        // {
        //     int currNode = 0;
            
        //     while(currNode < currentPath.Count - 1)
        //     {
        //         Vector3 start = map.CellCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
        //         Vector3 end = map.CellCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);

        //         Debug.DrawLine(start, end, Color.red);
        //         currNode++;
        //     }
        // }
        
        // 이동 처리
        if(isMoving && currentPath != null)
        {
            MoveAlongPath();
        }
    }
    
    // 경로 이동 시작
    public void StartMoving(Vector3Int targetPos)
    {
        if (accessibleTiles.ContainsKey(targetPos)) 
        {
            // 타일맵 청소
            TileMapManager.Instance.ClearTileMap(accessibleTiles.Keys);
            //이동
            currentPath = accessibleTiles[targetPos];
            currentPathIndex = 0;
            isMoving = true;
        }
        else
        {
            print("이동 가능한 타일이 아닙니다.");
        }
    }

    private bool RandomFunc(int x)
    {
        int ran = Random.Range(0, 100);
        
        return ran < x;
    }
    public void Attack(Unit enemy)
    {
        TileMapManager.Instance.ClearTileMap(attackableTiles);
        int damage = this.atk;
        
        // 명중률 계산
        int accuracy = 85 + 4 * (foc - enemy.foc) -3*((int)Mathf.Round(Vector3.Distance(cellPosition,enemy.cellPosition)) - 2);
        if (!RandomFunc(accuracy))
        {
            print("빗나감 ㅅㄱ");
            GameManager.Instance.UpdateBattleState(BattleState.Next); // 공격 완료 후 Next로 전환
            return;
        }
        
        //치명타 계산
        int crit = 10 + 5 * foc;
        if(RandomFunc(crit)) damage += 2;
        
        //공격횟수 계산
        int attackCount = 0;
        if (spd < 5) attackCount = 1;
        else if (spd < 8) attackCount = 2;
        else if (spd < 10) attackCount = 3;
        else attackCount = 4;
        
        enemy.Attacked(damage,attackCount);
    }

    public void Attacked(int damage, int attackCount)
    {
        for (int i = 0; i < attackCount; i++)
        {
            hp -= damage;

            if (hp <= 0)
            {
                UnitManager.Instance.UnitEliminate(this);
                break;
            }
        }
        GameManager.Instance.UpdateBattleState(BattleState.Next);// 공격 완료 후 Next로 전환
    }
    
    // 경로를 따라 이동
    void MoveAlongPath()
    {
        if(currentPathIndex >= currentPath.Count)
        {
            // 모든 경로 이동 완료
            isMoving = false;
            currentPath = null;
            GameManager.Instance.UpdateBattleState(BattleState.Default);// 이동 완료후 Default로 전환
            return;
        }
        
        Vector3Int targetCellPos = new Vector3Int(currentPath[currentPathIndex].x,currentPath[currentPathIndex].y,currentPath[currentPathIndex].z);
        // 목표 위치 계산
        Vector3 targetPosition = TileMapManager.Instance.CellCoordToWorldCoord(targetCellPos);
        
        // 현재 위치에서 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // 목표 지점 도착 확인
        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // 정확한 위치로 스냅
            transform.position = targetPosition;
            
            // 타일 좌표 업데이트
            cellPosition = targetCellPos;
            
            // 다음 노드로 이동
            currentPathIndex++;
        }
    }
}