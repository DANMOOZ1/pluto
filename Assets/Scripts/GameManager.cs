using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;
    public BattleState  battleState;

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
                HandleBattle();
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

    private void HandleBattle()
    {
        UpdateBattleState(BattleState.Move);
    }

    public void UpdateBattleState(BattleState newBattleState)
    {
        battleState = newBattleState;

        switch (battleState)
        {
            case BattleState.Move:
                break;
            case BattleState.Combat:
                break;
            case BattleState.Next:
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
    Move,
    Combat,
    Next,
    EnemyTurn
}