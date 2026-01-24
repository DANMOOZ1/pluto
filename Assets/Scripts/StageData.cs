using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

[Serializable]
public class StageData
{
    public SerializedDictionary<int, List<UnitEntry>> WavesInStage = new SerializedDictionary<int, List<UnitEntry>>();
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
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
            
        UnitEntry other = (UnitEntry)obj;
        return position[0] == other.position[0] &&
               position[1] == other.position[1] &&
               position[2] == other.position[2] &&
               unitName == other.unitName &&
               isAlly == other.isAlly;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(position[0], position[1], position[2], unitName, isAlly);
    }
}