using System.Data;
using UnityEngine;

public enum StatType { HP, ATK, DEF, SPD, FOC}
public enum BarType { small, big }

public class StatFrame : MonoBehaviour
{
    public StatType statType;
    public BarType barType;
}
