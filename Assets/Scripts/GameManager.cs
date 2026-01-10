using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<UnitSO> units = new List<UnitSO>();
    
    public GameObject selectedUnit;
    public List<GameObject> AccessibleUnits = new List<GameObject>();
        
    void Awake()
    {
        foreach (UnitSO u in units)
        {
            UnitCreater(u);
        }
        
        selectedUnit = AccessibleUnits[0];
    }

    public void UnitCreater(UnitSO unitData)
    {
        GameObject unitObject = new GameObject(unitData.unitName);
        
        Unit unit = unitObject.AddComponent<Unit>();
        SpriteRenderer renderer = unitObject.AddComponent<SpriteRenderer>();

        unit.hp = unitData.hp;
        unit.atk = unitData.atk;
        unit.def = unitData.def;
        unit.wil =  unitData.wil;
        unit.mnt = unitData.mnt;
        unit.spd = unitData.spd;
        unit.foc = unitData.foc;
        unit.rng = unitData.rng;
        unit.hta = unitData.hta;
        unit.race = unitData.race;
        unit.unitName = unitData.unitName;
        unit.sprite = unitData.sprite;
        
        AccessibleUnits.Add(unitObject);

        renderer.sprite = unit.sprite;
    }
}
