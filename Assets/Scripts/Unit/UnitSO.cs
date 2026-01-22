using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public int def;
    public int spd;
    public int foc;
    public Sprite movImage;
    public Sprite rng;
    public Sprite hta;
    public int mov;
    public int level;
    public Sprite portrait;
    public TileCheckRule movementRule;
    public TileCheckRule atkRule;
    public HeatAreaRule heatAreaRule;
}

