using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    //등록된 유닛 프리팹
    public List<UnitSO> units = new List<UnitSO>();
    
    public GameObject selectedUnit;
    public int selectedUnitIndex = 0;
    public List<GameObject> accessibleUnits = new List<GameObject>();
        
    void Awake()
    {
        //디버깅 용으로 유닛 생성
        Vector3Int pos = new Vector3Int(0,1,0);
        foreach (UnitSO u in units)
        {
            
            UnitCreater(u,pos);
        }
        
        selectedUnit = accessibleUnits[selectedUnitIndex];

        GameManager.Instance.OnBattleStateChange += UnitMove;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnBattleStateChange -= UnitMove;
    }

    private void UnitMove()
    {
        /*//사전 데이터 수집   
        Vector3Int _pos = selectedUnit.GetComponent<UnitController>().cellPosition;
        int _mov = selectedUnit.GetComponent<Unit>().mov;
        MovementRule _movementRule = selectedUnit.GetComponent<Unit>().movementRule;
        
        //이동가능한 영역 표시 타일을 만들고 이동가능한 타일을 반환함
        selectedUnit.GetComponent<UnitController>().accessibleTiles = TileMapManager.Instance.ReachableTile(_pos, _mov, _movementRule);*/
    }

    public void UnitCreater(UnitSO unitData, Vector3Int unitCellPos)
    {
        GameObject unitObject = new GameObject(unitData.unitName);
        
        // 유닛 오브젝트에 컴포넌트 추가
        Unit unit = unitObject.AddComponent<Unit>();
        SpriteRenderer renderer = unitObject.AddComponent<SpriteRenderer>();

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
        unit.isAlly = unitData.isAlly;
        unit.portrait = unitData.portrait;
        unit.movementRule =  unitData.movementRule;
        unit.cellPosition = unitCellPos;
        
        renderer.sprite = unit.sprite;
        renderer.sortingOrder = 10;
        
        // 사용가능한 유닛리스트에 만든 유닛 오브젝트 저장
        accessibleUnits.Add(unitObject);
    }

    public void UnitChange()
    {
        if (accessibleUnits.Count > selectedUnitIndex) selectedUnitIndex++;
    }
}
