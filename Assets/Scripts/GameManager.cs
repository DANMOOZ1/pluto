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
        
        unit.atk =  unitData.atk;
        unit.hp = unitData.hp;
        unit.race = unitData.race;
        unit.unitName = unitData.unitName;
        unit.sprite = unitData.sprite;
        
        AccessibleUnits.Add(unitObject);
    }
}
