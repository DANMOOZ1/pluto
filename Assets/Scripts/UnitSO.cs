using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public Sprite sprite;
}

