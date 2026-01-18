using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    //등록된 유닛 프리팹
    public List<UnitSO> units = new List<UnitSO>();
    
    public Unit selectedUnit;// 아군 적군
    public Unit selectedEnemy;
    public int selectedUnitIndex = 0;
    public List<Unit> accessibleUnits = new List<Unit>();//아군
    public List<Unit> enemyUnits = new List<Unit>();//적군
    public List<Unit> spdSortUnits = new List<Unit>();
    
    void Awake()
    {
        // 아군 생성
        int i = 0;
        Vector3Int pos =  new Vector3Int(-10,4,0);
        foreach (UnitSO u in units)
        {
            accessibleUnits.Add(UnitCreater(u,pos, true));
            i++;
            break;
        }
        
        //적군생성
        i = 0;
        pos = new Vector3Int(-1,5,0);
        foreach (UnitSO u in units)
        {
            enemyUnits.Add(UnitCreater(u, pos, false));
            i++;
            pos.y += i;
        }
        
        //구독
        GameManager.Instance.OnBattleStateChange += UnitMovePrepare;
        GameManager.Instance.OnBattleStateChange += UnitAttackPrepare;
        GameManager.Instance.OnBattleStateChange += TurnSetting;
        GameManager.Instance.OnBattleStateChange += NextUnit;
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            //구독 취소 : 램 누수 방지
            GameManager.Instance.OnBattleStateChange -= UnitMovePrepare;
            GameManager.Instance.OnBattleStateChange -= UnitAttackPrepare;
            GameManager.Instance.OnBattleStateChange -= TurnSetting;
            GameManager.Instance.OnBattleStateChange -= NextUnit;
        }
    }

    //다음 유닛으로 이동 혹은 승패 여부를 결정
    public void NextUnit()
    {
        if (GameManager.Instance.battleState == BattleState.Next)
        {
            //승패 판정
            if (accessibleUnits.Count == 0) GameManager.Instance.UpdateGameState(GameState.Defeat);
            else if (enemyUnits.Count == 0) GameManager.Instance.UpdateGameState(GameState.Victory);
            else
            {
                if(selectedUnitIndex < spdSortUnits.Count-1) selectedUnitIndex++;
                else selectedUnitIndex = 0;
                selectedUnit = spdSortUnits[selectedUnitIndex];
                
                GameManager.Instance.UpdateBattleState(BattleState.Move);
            }
            
        }
    }
    
    //유닛의 턴 순서 정하는 함수, BattleState : Setting
    private void TurnSetting()
    {
        if (GameManager.Instance.battleState == BattleState.Setting)
        {
            // 1. 두 리스트를 하나로 합치기
            List<Unit> combinedList = new List<Unit>();
            combinedList.AddRange(accessibleUnits);
            combinedList.AddRange(enemyUnits);

            // 2. 합병 정렬 수행
            spdSortUnits = MergeSort(combinedList);
    
            selectedUnit = spdSortUnits[0];
            selectedUnitIndex = 0;
            
            foreach (Unit u in spdSortUnits) print(u.unitName);
            
            GameManager.Instance.UpdateBattleState(BattleState.Move);
        }
    }

    private List<Unit> MergeSort(List<Unit> list)
    {
        // 기저 조건: 리스트 크기가 1 이하면 이미 정렬됨
        if (list.Count <= 1) return list;

        // 리스트를 반으로 나누기
        int mid = list.Count / 2;
        List<Unit> left = list.GetRange(0, mid);
        List<Unit> right = list.GetRange(mid, list.Count - mid);

        // 재귀적으로 정렬
        left = MergeSort(left);
        right = MergeSort(right);

        // 병합
        return Merge(left, right);
    }

    private List<Unit> Merge(List<Unit> left, List<Unit> right)
    {
        List<Unit> result = new List<Unit>();
        int i = 0;
        int j = 0;

        // 두 리스트를 비교하며 병합
        while (i < left.Count && j < right.Count)
        {
            // ShouldAddAllyUnit 대신 일반적인 비교 함수 사용
            if (ShouldAddFirst(left[i], right[j]))
            {
                result.Add(left[i]);
                i++;
            }
            else
            {
                result.Add(right[j]);
                j++;
            }
        }

        // 남은 요소들 추가
        while (i < left.Count)
        {
            result.Add(left[i]);
            i++;
        }

        while (j < right.Count)
        {
            result.Add(right[j]);
            j++;
        }

        return result;
    }

    private bool ShouldAddFirst(Unit unit1, Unit unit2)
    {
        // 속도가 다르면 속도가 빠른 것이 먼저
        if (unit1.spd != unit2.spd)
        {
            return unit1.spd > unit2.spd;
        }

        // 속도가 같으면 레벨이 높은 것이 먼저
        if (unit1.level != unit2.level)
        {
            return unit1.level > unit2.level;
        }

        // 속도와 레벨이 모두 같으면 아군 우선
        // unit1이 아군이고 unit2가 적이면 unit1을 먼저
        // unit1이 적이고 unit2가 아군이면 unit2를 먼저 (false 반환)
        if (unit1.isAlly != unit2.isAlly)
        {
            return unit1.isAlly;
        }

        // 둘 다 아군이거나 둘 다 적이면 순서 유지
        return true;
    }
    
    //유
    private void UnitMovePrepare()
    {
        //state  체크
        switch (GameManager.Instance.battleState)
        {
            case BattleState.Move:
                Unit unit = selectedUnit.GetComponent<Unit>();
                if (unit.isAlly)
                {
                    Vector3Int pos = unit.cellPosition;
                    int mov = unit.mov;
                    MovementRule movementRule = unit.movementRule;
                    
                    //유닛이 갈 수 있는 타일을 변수에 저장 및 sublayer tilemap에 표시
                    unit.accessibleTiles = TileMapManager.Instance.ReachableTile(pos, mov, movementRule);
                }
                else
                {
                    //적 AI 가동
                     GameManager.Instance.UpdateBattleState(BattleState.Next);
                    
                }
                break;
        }
    }
    
    private void UnitAttackPrepare()
    {
        //state  체크
        switch (GameManager.Instance.battleState)
        {
            case BattleState.Combat:
                Unit unit = selectedUnit.GetComponent<Unit>();
                Vector3Int pos = unit.cellPosition;
                AttackRule atkRule = unit.atkRule;
                
                //유닛이 갈 수 있는 타일을 변수에 저장 및 sublayer tilemap에 표시
                unit.attackableTiles = TileMapManager.Instance.AttackableTile(pos, atkRule);
                
                break;
        }
    }

    public GameObject unitPrefab;
    public Unit UnitCreater(UnitSO unitData, Vector3Int unitCellPos, bool isAlly)
    {
        GameObject unitObject = Instantiate(unitPrefab,TileMapManager.Instance.CellCoordToWorldCoord(unitCellPos),Quaternion.identity);
        
        // 유닛 오브젝트에 컴포넌트 추가
        Unit unit = unitObject.GetComponent<Unit>();
        SpriteRenderer renderer = unitObject.GetComponent<SpriteRenderer>();

        //컴포넌트에 데이터 삽입
        unit.hp = unitData.hp;
        unit.atk = unitData.atk;
        unit.def = unitData.def;
        unit.spd = unitData.spd;
        unit.foc = unitData.foc;
        unit.rng = unitData.rng;
        unit.hta = unitData.hta;
        unit.race = unitData.race;
        unit.unitName = unitData.unitName;
        unit.sprite = unitData.sprite;
        unit.mov = unitData.mov;
        unit.level = unitData.level;
        unit.isAlly = isAlly;
        unit.portrait = unitData.portrait;
        unit.cellPosition = unitCellPos;
        unit.movementRule =  unitData.movementRule;
        unit.atkRule = unitData.atkRule;
        
        renderer.sprite = unit.sprite;
        renderer.sortingOrder = 10;
        
        unitObject.name = unit.unitName;
        
        // 사용가능한 유닛리스트에 만든 유닛 저장
        return unit;
    }

    public void UnitEliminate(Unit unit)
    {
        //리스트에서 Unit 제거
        if (unit.isAlly) accessibleUnits.Remove(unit);
        else enemyUnits.Add(unit);
        spdSortUnits.Remove(unit);

        Destroy(unit.gameObject);
    }
}
