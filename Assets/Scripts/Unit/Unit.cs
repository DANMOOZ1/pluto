using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

//[RequireComponent(typeof(PolygonCollider2D))]
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
    public Sprite movImage;
    public Sprite rng;
    public Sprite hta;
    public int level;
    public bool isAlly;
    public Sprite portrait;
    public TileCheckRule movementRule;
    public TileCheckRule atkRule;
    public HeatAreaRule heatAreaRule;
    public UnitSO unitSO;
    
    //인게임 유닛 테이터
    public Vector3Int cellPosition; // cell 좌표임
    public List<Node> currentPath = null;//이동을 시작할때 목표 타일까지의 경로를 나타냄
    public List<Vector3Int> accessibleTiles = new List<Vector3Int>();// 현재 시점에서 이동 가능한 타일을 나타냄
    public List<Vector3Int> attackableTiles = new List<Vector3Int>();// 현재 시점에서 공격 가능한 타일을 나타냄
    public float moveSpeed = 5f;  // 이동 속도
    private int currentPathIndex = 0;  // 현재 목표 노드 인덱스
    public bool isMoving = false;  // 이동 중인지 확인

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
        if(UnitManager.Instance.UnitOnTile(targetPos) && targetPos != cellPosition){
            print("해당 위치에 이미 유닛이 존재합니다.");
            return;
        }
        if (isAlly && accessibleTiles.Contains(targetPos)) 
        {
            // 타일맵 청소
            TileMapManager.Instance.ClearTileMap(accessibleTiles);
            //이동
            currentPath = TileMapManager.Instance.GeneratePathTo(cellPosition,targetPos, movementRule);
            currentPathIndex = 0;
            isMoving = true;
        }
        else if(!isAlly)
        {
            //이동
            currentPath = TileMapManager.Instance.GeneratePathTo(cellPosition,targetPos, movementRule);
            currentPathIndex = 0;
            isMoving = true;
        }
        else
        {
            print("이동 가능한 경로가 존재하지 않습니다.");
        }

    }

    // 경로를 따라 이동
    void MoveAlongPath()
    {
        if(currentPathIndex >= currentPath.Count)
        {
            // 모든 경로 이동 완료
            isMoving = false;
            currentPath = null;
            
            if(isAlly) GameManager.Instance.UpdateBattleState(BattleState.Default);// 아군 유닛의 경우 이동 완료후 Default로 전환
            else GameManager.Instance.UpdateBattleState(BattleState.Combat); // 적 유닛의 경우 이동 완료후  combat으로 전환
            return;
        }
        
        Vector3Int targetCellPos = new Vector3Int(currentPath[currentPathIndex].x,currentPath[currentPathIndex].y,currentPath[currentPathIndex].z);
        // 목표 위치 계산
        Vector3 targetPosition = TileMapManager.Instance.CellCoordToWorldCoord(targetCellPos);
        //반블록일시 위치를 맞춰줌
        if(TileMapManager.Instance.dataOnTiles[targetCellPos].escalator) targetPosition += new Vector3(0,-0.5f,0);
        
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
    
    //1~100 사이의 확률을 받아 bool 출력
    private bool RandomFunc(int x)
    {
        int ran = Random.Range(0, 100);
        
        return ran < x;
    }
    
    // enemy에 대한 공격 연산 및 타일맵 클리어
    public void Attack(Unit enemy)
    {
        TileMapManager.Instance.ClearTileMap(attackableTiles);

        List <Unit> enemies = CheckEnemyOnHeatArea(cellPosition, enemy.cellPosition);
        foreach (Unit enem in enemies)
        {
            int damage = atk;
        
            // 명중률 계산
            int accuracy = 85 + 4 * (foc - enem.foc) -3*((int)Mathf.Round(Vector3.Distance(cellPosition,enem.cellPosition)) - 2);
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
        
            enem.Attacked(damage,attackCount);
        }
        
    }

    public void Attacked(int damage, int attackCount)
    {
        for (int i = 0; i < attackCount; i++)
        {
            hp -= damage;
            print(unitName+"(이)가"+damage+"만큼 피해를 입음");
            if (hp <= 0)
            {
                hp = 0;
                UnitManager.Instance.UnitEliminate(this);
                break;
            }
        }
        GameManager.Instance.UpdateBattleState(BattleState.Next);// 공격 완료 후 Next로 전환
    }

    private List<Unit> CheckEnemyOnHeatArea(Vector3Int currPos,Vector3Int targetPos)
    {
        List<Vector3Int> heatArea = heatAreaRule.HeatAreaRuleFunc(currPos, targetPos);

        print(heatArea);
        List<Unit> attackedUnits = new List<Unit>();
        if (heatArea.Count > 0)
        {
            foreach (Vector3Int heatPos in heatArea)
            {
                foreach (Unit u in UnitManager.Instance.spdSortUnits)
                {
                    if (heatPos == u.cellPosition) attackedUnits.Add(u);
                }
            }
            
            if(attackedUnits.Count > 0) return attackedUnits;
        }
        Debug.LogWarning("HeatArea에 감지된 유닛이 없습니다.");
        return attackedUnits;
    }

}