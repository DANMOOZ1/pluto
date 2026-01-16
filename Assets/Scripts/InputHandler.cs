using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    private Camera _mainCamera;
    public int zLayerCount;
    private void Awake(){
        _mainCamera = Camera.main;
        zLayerCount = TileMapManager.Instance.tilemaps.Length;
    }

    public void OnClick(InputAction.CallbackContext context){
        if(!context.started) return;

        switch (GameManager.Instance.battleState)
        {
            case BattleState.Default:
                break;
            case BattleState.Move:
                UnitMove();
                break;
            case BattleState.Combat:
                UnitAttack();
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

        switch (GameManager.Instance.combatState)
        {
            case CombatState.EnemySelecting:
                selectEnemy();
                break;
            case CombatState.Attack:
                break;
        }
    }

    //마우스 위치에 해당하는 cellpos를 출력, 해당하는 타일이 없으면 null 출력
    public Vector3Int? MousePosToCellPos()
    {
        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        int i = 0;
        Vector3Int? foundPos = null;
    
        foreach (var tilemap in TileMapManager.Instance.tilemaps)
        {
            Vector3Int mousePosTranslated = tilemap.WorldToCell(mousePos);
            mousePosTranslated = mousePosTranslated - new Vector3Int(5, 5, 0); // 왜인진 모르겠는데 isometric z as y 로 설정하면 x,y cell pos가 +5 됨
            mousePosTranslated.z = i;
        
            if (TileMapManager.Instance.dataOnTiles.ContainsKey(mousePosTranslated))
            {
                foundPos = mousePosTranslated;
                // 계속 순회하면서 더 높은 z값을 가진 타일을 찾음
            }
            i++;
        }

        return foundPos;
    }

    public void UnitAttack()
    {
        Vector3Int? cellpos = MousePosToCellPos();

        if (cellpos.HasValue)
        {
            UnitManager.Instance.selectedUnit.GetComponent<Unit>().AttackEnemy();
        }
        else
        {
            print("공격 가능한 타일이 아닙니다.");
        }
    }
    public void UnitMove()
    {
        Vector3Int? cellpos = MousePosToCellPos();

        if (cellpos.HasValue)
        {
            UnitManager.Instance.selectedUnit.GetComponent<Unit>().StartMoving(cellpos.Value);
        }
        else
        {
            print("이동 가능한 타일이 아닙니다.");
        }
    }

    public void selectEnemy()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<Unit>() != null)
        {
            Unit clickedUnit =  hit.collider.GetComponent<Unit>();

            if (!clickedUnit.isAlly)
            {
                UIManager.Instance.selectedEnemy = clickedUnit;
            }
        }

        GameManager.Instance.UpdateCombatState(CombatState.EnemySelecting);
    }


    // 버튼에 들어갈 함수
    public void MoveButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Move);
    }

    public void CombatButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Combat);
    }

    public void NextButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Next);
    }

    public void InfoButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Info);
    }
}
