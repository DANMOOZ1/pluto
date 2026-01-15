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
                // button�� ���� ���� + Default�� ������ HandleBattle�� �ʿ���� �� ���Ҵµ�.. ���ð� �ʿ��ϸ� �츮����
                // button�� �� �Լ����� UIManager�� �־�ξ����ϴ�
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
                
                //이동가능한 영역 선택타일 생성및 그 리스트를 할당
                Vector3Int pos = UnitManager.Instance.selectedUnit.GetComponent<Unit>().cellPosition;
                int mov = UnitManager.Instance.selectedUnit.GetComponent<Unit>().mov;
                MovementRule movementRule = UnitManager.Instance.selectedUnit.GetComponent<Unit>().movementRule;
                
                UnitManager.Instance.selectedUnit.GetComponent<Unit>().accessibleTiles = TileMapManager.Instance.ReachableTile(pos, mov, movementRule);
                
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