using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

[Serializable]
public class StageData
{
    public List<UnitEntry> UnitsInStage = new List<UnitEntry>();
}

[Serializable]
public class UnitEntry
{
    public int[] position;
    public string unitName;
    public bool isAlly;
    
    public UnitEntry(int[] pos, string name, bool isAy)
    {
        position = pos;
        unitName = name;
        isAlly = isAy;
        
    }
}