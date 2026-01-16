using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    //등록된 유닛 프리팹
    public List<UnitSO> units = new List<UnitSO>();
    
    public GameObject selectedUnit;// 아군 적군
    public Unit selectedEnemy;
    public int selectedUnitIndex = 0;
    public List<GameObject> accessibleUnits = new List<GameObject>();//아군
    public List<GameObject> enemyUnits = new List<GameObject>();//적군
        
    
    void Awake()
    {
        // 아군 생성
        int i = 0;
        Vector3Int pos = new Vector3Int(i,1,0);
        foreach (UnitSO u in units)
        {
            accessibleUnits.Add(UnitCreater(u,pos, true));
            i++;
        }
        
        selectedUnit = accessibleUnits[selectedUnitIndex];
        
        //적군생성
        i = 0;
        pos = new Vector3Int(i,0,0);
        foreach (UnitSO u in units)
        {
            enemyUnits.Add(UnitCreater(u,pos, false));
            i++;
        }
        
        //구독
        GameManager.Instance.OnBattleStateChange += UnitMovePrepare;
        GameManager.Instance.OnBattleStateChange += UnitAttackPrepare;
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            //구독 취소 : 램 누수 방지
            GameManager.Instance.OnBattleStateChange -= UnitMovePrepare;
            GameManager.Instance.OnBattleStateChange -= UnitAttackPrepare;
        }
    }

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
                    if (accessibleUnits.Count > selectedUnitIndex) selectedUnitIndex++;
                    else selectedUnitIndex = 0;
                    GameManager.Instance.UpdateBattleState(BattleState.Move);
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
    public GameObject UnitCreater(UnitSO unitData, Vector3Int unitCellPos, bool isAlly)
    {
        //규리님 원래 빈 오브젝트를 생성하는 것에서 prefab 생성으로 바꿨어요! 컴포넌트 추가를 원하시면 프리팹에서 하시면 될 것 같습니다.
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
        
        // 사용가능한 유닛리스트에 만든 유닛 오브젝트 저장
        return unitObject;
    }

    public void UnitChange()
    {
        if (accessibleUnits.Count > selectedUnitIndex) selectedUnitIndex++;
    }
}
