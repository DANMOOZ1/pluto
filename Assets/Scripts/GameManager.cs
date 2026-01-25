using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        print(gameState);

        GameObject managers = GameObject.Find("Managers");

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
            case GameState.Victory:
                if (managers != null)
                {
                    Destroy(managers);
                }
                SceneManager.LoadScene("Scenes/TitleScene");
                break;
            case GameState.Defeat:
                if (managers != null)
                {
                    Destroy(managers);
                }
                SceneManager.LoadScene("Scenes/TitleScene");
                break;
            case GameState.Debug:
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
    UnitSetUp,
    Victory,
    Defeat,
    Debug
    
}

public enum BattleState
{
    Setting,
    Default,
    Move,
    Combat,
    Next,
    Info
}
