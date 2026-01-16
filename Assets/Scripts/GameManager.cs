using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;
    public BattleState  battleState;
    public CombatState combatState;

    public event Action OnGameStateChange;
    public event Action OnBattleStateChange;
    public event Action OnCombatStateChange;
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
                UpdateBattleState(BattleState.Default);
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
                break;
            case BattleState.Move:
                break;
            case BattleState.Combat:
                UpdateCombatState(CombatState.EnemySelecting);
                break;
            case BattleState.Next:
                break;
            case BattleState.Info:
                break;
            case BattleState.EnemyTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        OnBattleStateChange?.Invoke();
    }

    public void UpdateCombatState(CombatState newCombatState)
    {
        combatState = newCombatState;

        switch (combatState)
        {
            case CombatState.EnemySelecting:
                break;
            case CombatState.Attack:
                break;
        }
        OnCombatStateChange?.Invoke();
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
    Default,
    Move,
    Combat,
    Next,
    Info,
    EnemyTurn
}

public enum CombatState
{
    EnemySelecting,
    Attack
}