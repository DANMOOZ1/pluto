using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public int def;
    public int wil;
    public int mnt;
    public int spd;
    public int foc;
    public int rng;
    public int hta;
    public Sprite sprite;
}

