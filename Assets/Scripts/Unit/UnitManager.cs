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
        if (GameManager.HasInstance) //오류나서 gemini한테 물어보니까 추가하라고 해서 자아 없이 추가했습니다.. 문제 있음 알려주세용..
        {
            GameManager.Instance.OnBattleStateChange -= UnitMove;
        }
    }

    private void UnitMove()
    {
        Vector3Int _pos = selectedUnit.GetComponent<UnitController>().cellPosition;
        int _mov = selectedUnit.GetComponent<Unit>().mov;
        
        //TileMapManager.Instance.ReachableTile(_pos, _mov);
    }

    public void UnitCreater(UnitSO unitData, Vector3Int unitCellPos)
    {
        GameObject unitObject = new GameObject(unitData.unitName);
        
        // 유닛 오브젝트에 컴포넌트 추가
        Unit unit = unitObject.AddComponent<Unit>();
        UnitController unitController =  unitObject.AddComponent<UnitController>();
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
         
        unitController.cellPosition = unitCellPos;
        
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
