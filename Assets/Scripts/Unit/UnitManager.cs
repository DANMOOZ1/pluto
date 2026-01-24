using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class UnitManager : Singleton<UnitManager>
{
    //등록된 유닛 프리팹
    public SerializedDictionary<string, GameObject> units;
    public Unit selectedUnit;// 아군 적군
    public int selectedUnitIndex = 0;
    public List<Unit> allyUnits = new List<Unit>();//아군
    public List<Unit> enemyUnits = new List<Unit>();//적군
    public List<Unit> spdSortUnits = new List<Unit>();
    
    //For UIManager and InputHandler
    public Unit selectedAlly; // UI에서 "현재 차례는 아니지만 선택된 아군"이 필요해져서 추가했습니다.
    public Unit selectedEnemy;
    
    
    void Awake()
    { 
        
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
    
    public void GenerateUnitsByEntryList(List<UnitEntry> unitEntryList)
    {
        foreach (UnitEntry unitEntry in unitEntryList)
        {
            GenerateUnit(unitEntry);
        }
    }

    public void GenerateUnit(UnitEntry unitEntry)
    {
        if (units.ContainsKey(unitEntry.unitName))
        {
            Vector3Int pos = new Vector3Int(unitEntry.position[0], unitEntry.position[1], unitEntry.position[2]);
                
            if (unitEntry.isAlly) allyUnits.Add(UnitCreater(units[unitEntry.unitName], pos, unitEntry.isAlly));
            else enemyUnits.Add(UnitCreater(units[unitEntry.unitName], pos, unitEntry.isAlly));
                
        }
        else Debug.LogWarning(unitEntry.unitName+": 해당 이름을 지닌 유닛이 딕셔너리에 존재하지 않습니다.");
    }

    //다음 유닛으로 이동 혹은 승패 여부를 결정
    public void NextUnit()
    {
        if (GameManager.Instance.battleState == BattleState.Next)
        {
            //승패 판정
            if (allyUnits.Count == 0) GameManager.Instance.UpdateGameState(GameState.Defeat);
            else if (enemyUnits.Count == 0)
            {
                int currWaveIndex = DataManager.Instance.waveIndex;
                
                //남은 웨이브가 있는 경우 해당 웨이브의 유닛을 소환
                if (DataManager.Instance.StageData.WavesInStage.ContainsKey(currWaveIndex + 1))
                {
                    //빈 웨이브의 경우 return
                    if (DataManager.Instance.StageData.WavesInStage[currWaveIndex + 1].Count == 0) return;
                        
                    GenerateUnitsByEntryList(DataManager.Instance.StageData.WavesInStage[currWaveIndex + 1]);
                    print(currWaveIndex + 1+"번쨰 웨이브 소환");
                    DataManager.Instance.waveIndex = currWaveIndex + 1;
                    TurnSetting(); // 버그 가능성 높음
                    
                } else GameManager.Instance.UpdateGameState(GameState.Victory);//남아 있는 웨이브가 없는 경우 Victory로 전환
            }
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
            combinedList.AddRange(allyUnits);
            combinedList.AddRange(enemyUnits);

            // 2. 합병 정렬 수행
            spdSortUnits = MergeSort(combinedList);
    
            selectedUnit = spdSortUnits[0];
            selectedUnitIndex = 0;
            
            //foreach (Unit u in spdSortUnits) print(u.unitName);
            
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
                Unit unit = selectedUnit;
                
                if (unit.isAlly)
                {
                    //아군 유닛인 경우
                    Vector3Int pos = unit.cellPosition;
                    TileCheckRule movementRule = unit.movementRule;
                    
                    //유닛이 갈 수 있는 타일을 변수에 저장 및 sublayer tilemap에 표시 
                    unit.accessibleTiles = TileMapManager.Instance.ReachableTile(pos, movementRule);
                }
                else
                {
                    //적 유닛인 경우
                    unit.gameObject.GetComponent<EnemyAI>().Move();
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
                Unit unit = selectedUnit;

                if (unit.isAlly)
                {
                    //아군 유닛인 경우
                    Vector3Int pos = unit.cellPosition;
                    TileCheckRule atkRule = unit.atkRule;
                
                    //유닛이 공격할 수 있는 타일을 변수에 저장 및 sublayer tilemap에 표시
                    unit.attackableTiles = TileMapManager.Instance.AttackableTile(pos, atkRule);
                }
                else
                {
                    Unit targetUnit = selectedUnit.gameObject.GetComponent<EnemyAI>().target;
                    
                    //공격이 불가능하면 넘어감
                    if(selectedUnit.gameObject.GetComponent<EnemyAI>().canAttack) unit.Attack(targetUnit);
                    else GameManager.Instance.UpdateBattleState(BattleState.Next);
                }
                break;
        }
    }

    // selected Unit의 공격 범위 내에 공격 가능한 적이 있는지 출력
    public bool UnitCanAttack()
    {
        List<Vector3Int> attackableTiles =
            TileMapManager.Instance.ReturnInteractiveTiles(selectedUnit.cellPosition, selectedUnit.atkRule);
        foreach (Unit u in enemyUnits)
        {
            if (attackableTiles.Contains(u.cellPosition)) return true;
        }
        return false;
    }

    public bool UnitOnTile(Vector3Int cellPosition)
    {
        foreach (Unit u in spdSortUnits)
        {
            if (u.cellPosition == cellPosition) return true;
        }
        return false;
    }
    
    public Unit UnitCreater(GameObject unitPrefab, Vector3Int unitCellPos, bool isAlly)
    {
        GameObject unitObject = Instantiate(unitPrefab,TileMapManager.Instance.CellCoordToWorldCoord(unitCellPos),Quaternion.identity);
        
        // 유닛 오브젝트에 컴포넌트 추가
        Unit unit = unitObject.GetComponent<Unit>(); // 가변값
        
        UnitDataReset(unit);
        unit.cellPosition = unitCellPos;
        unit.isAlly = isAlly;

        // UI에서 필요해서 넣었는데 필요 없어지면 뺄게요 얘가 자식 오브젝트들의 순서를 잘 정리해주기를 바라고 있습니다..
        var sg = unitObject.AddComponent<UnityEngine.Rendering.SortingGroup>();
        sg.sortingOrder = 10;

        // 사용가능한 유닛리스트에 만든 유닛 저장
        return unit;
    }

    public void UnitEliminate(Unit unit)
    {
        //인덱스 당겨오기
        if (spdSortUnits.IndexOf(unit) < selectedUnitIndex) selectedUnitIndex -= 1;
        //리스트에서 Unit 제거    
        if (unit.isAlly) allyUnits.Remove(unit);
        else enemyUnits.Add(unit);
        spdSortUnits.Remove(unit);

        Destroy(unit.gameObject);
    }

    //유닛의 실제 데이터를 SO에서 불러와 초기화 시킴
    private void UnitDataReset(Unit unit)
    {
        UnitSO unitData = unit.unitSO; //초기값
        
        //컴포넌트에 데이터 삽입
        unit.hp = unitData.hp;
        unit.atk = unitData.atk;
        unit.def = unitData.def;
        unit.spd = unitData.spd;
        unit.foc = unitData.foc;
        unit.movImage = unitData.movImage;
        unit.rng = unitData.rng;
        unit.hta = unitData.hta;
        unit.race = unitData.race;
        unit.unitName = unitData.unitName;
        unit.level = unitData.level;
        unit.portrait = unitData.portrait;
        unit.movementRule =  unitData.movementRule;
        unit.atkRule = unitData.atkRule;
        unit.heatAreaRule = unitData.heatAreaRule;
    }
}
