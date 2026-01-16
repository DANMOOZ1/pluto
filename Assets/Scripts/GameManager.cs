using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;
    public BattleState  battleState;
    public CombatState combatState;

    public event Action OnGameStateChange;
    public event Action OnBattleStateChange;
    private void Start()
    {
        UpdateGameState(GameState.Battle);
    }

    public void UpdateGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.Menu:
                break;
            case GameState.Battle:
                UpdateBattleState(BattleState.Move);
                break;
            case GameState.PositionSetUp:
                break;
            case GameState.UnitSetUp:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        OnGameStateChange?.Invoke();
    }

    public void UpdateBattleState(BattleState newBattleState)
    {
        battleState = newBattleState;

        switch (battleState)
        {
            case BattleState.Default:
                UIManager.Instance.UIBattle();
                break;
            case BattleState.Move:
                UIManager.Instance.UIMove();
                break;
            case BattleState.Combat:
                UpdateCombatState(CombatState.Default);
                break;
            case BattleState.Next:
                UIManager.Instance.UINext();
                break;
            case BattleState.Info:
                UIManager.Instance.UIInfo();
                break;
            case BattleState.EnemyTurn:
                UIManager.Instance.UIEnemyTurn();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        OnBattleStateChange?.Invoke();
    }

    // �̰� ���������� �𸣰ڳ׿�.. Unit.cs���� BattleState.Combat�� �� ���õ� Unit�� ���̸� CombatState.EnemySelected�� �ٲٶ�� �صξ����ϴ�.
    public void UpdateCombatState(CombatState newCombatState)
    {
        combatState = newCombatState;

        switch (combatState)
        {
            case CombatState.Default:
                UIManager.Instance.UICombat();
                break;
            case CombatState.EnemySelected:
                UIManager.Instance.UIEnemySelected();
                break;
            case CombatState.Attack:
                break;
        }
    }
}

public enum GameState
{
    Menu,
    Battle,
    PositionSetUp,
    UnitSetUp
    
}

public enum BattleState
{
    Default, // ���ư� ȭ���� �������� ����Ʈ�� �־�� �� �� ���Ҿ��
    Move,
    Combat,
    Next,
    Info,// ���� ���¿��� ����â�� ����� �ؼ� �߰��߽��ϴ�
    EnemyTurn
}

public enum CombatState
{
    Default,
    EnemySelected,
    Attack
}