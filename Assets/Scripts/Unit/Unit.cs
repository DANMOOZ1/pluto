using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public int def;
    public int spd;
    public int foc;
    public int rng;
    public int hta;
    public Sprite sprite;
    public int mov;
    public int level;
    public bool isAlly;
    public Sprite portrait;

    private void OnMouseDown()
    {
        if (isAlly)
        {
            if (GameManager.Instance.battleState == BattleState.Default)
            {
                UIManager.Instance.unit = this;
                UIManager.Instance.UIBattle();
            }
        } else
        {
            if (GameManager.Instance.battleState == BattleState.Combat)
            {
                UIManager.Instance.unit = this;
                GameManager.Instance.UpdateCombatState(CombatState.EnemySelected);
            }
        }
    }
}