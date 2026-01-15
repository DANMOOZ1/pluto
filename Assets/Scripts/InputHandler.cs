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

    public void UnitMove()
    {
        Vector3Int? cellpos = MousePosToCellPos();

        if (cellpos.HasValue)
        {
            UnitManager.Instance.selectedUnit.GetComponent<Unit>().StartMoving(cellpos.Value);
        }
        else
        {
            //"이동 불가능한 지역입니다" 알림하는 함수 위치
        }
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
