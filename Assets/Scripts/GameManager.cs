using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : SingletonPersistence<GameManager>
{
    public GameState gameState;
    public BattleState  battleState;

    public event Action OnGameStateChange;
    public event Action OnBattleStateChange;
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
                break;
            case GameState.Battle:
                break;
            case GameState.PositionSetUp:
                break;
            case GameState.UnitSetUp:
                break;
            case GameState.Victory:
                gameState = GameState.Menu;
                SceneManager.LoadScene("Scenes/TitleScene");
                break;
            case GameState.Defeat:
                gameState = GameState.Menu;
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
        OnBattleStateChange?.Invoke();
    }

    public void StageSetUp()
    {
        TileMapManager.Instance.GenerateTileData(); //tilemap을 읽고 graph를 생성
        DataManager.Instance.LoadUnits(); //stagedata(json)을 읽고 유닛을 생성(wave 1)
        UnitManager.Instance.TurnSetting(); //unitmanager의 Turnsetting으로 이어짐-> 게임 시작(Move)
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
    Default,
    Move,
    Combat,
    Next,
    Info
}
