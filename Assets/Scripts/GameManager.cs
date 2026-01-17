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
                UpdateBattleState(BattleState.Setting);
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
        print(battleState);
        switch (battleState)
        {
            case BattleState.Setting:
                break;
            case BattleState.Move:
                break;
            case BattleState.Default:
                break;
            case BattleState.Combat:
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
    Setting,
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