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
                //HandleBattle();
                // button에 의해 결정 + Default가 있으면 HandleBattle이 필요없을 것 같았는데.. 보시고 필요하면 살리세용
                // button에 들어갈 함수들은 UIManager에 넣어두었습니다
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


    //private void HandleBattle()
    //{
    //    UpdateBattleState(BattleState.Move);
    //}


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
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        OnBattleStateChange?.Invoke();
    }

    // 이게 적절할지는 모르겠네요.. Unit.cs에서 BattleState.Combat일 때 선택된 Unit이 적이면 CombatState.EnemySelected로 바꾸라고 해두었습니다.
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
    Default, // 돌아갈 화면이 있으려면 디폴트가 있어야 할 것 같았어요
    Move,
    Combat,
    Next,
    Info,// 정보 상태에서 정보창을 띄워야 해서 추가했습니다
    EnemyTurn
}

public enum CombatState
{
    Default,
    EnemySelected,
    Attack
}