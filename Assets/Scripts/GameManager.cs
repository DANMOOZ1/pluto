using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;
    public BattleState  battleState;

    public static event Action onGameStateChange;

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }

    public void UpdateGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.Menu:
                HandleMenu();
                break;
            case GameState.Battle:
                break;
            case GameState.PositionSetUp:
                break;
            case GameState.UnitSetUp:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        onGameStateChange?.Invoke();
    }

    private void HandleMenu()
    {
        return;
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