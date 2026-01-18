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
                InputAnalyze();
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

        switch (GameManager.Instance.combatState)
        {
            case CombatState.EnemySelecting:
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

    //유닛을 움직이는 명령인지 마우스 위치의 적을 선택한 것인지 판별함
    public void InputAnalyze()
    {
        GameObject obj = selectCollider();
    
        // 선택된 오브젝트가 없으면 이동
        if (obj == null)
        {
            UnitMove();
            return;
        }
    
        // 유닛 컴포넌트 확인
        Unit unit = obj.GetComponent<Unit>();
        if (unit == null)
            return;
    
        // 적군만 선택 처리
        if (!unit.isAlly)
        {
            UnitManager.Instance.selectedEnemy = unit;
            UIManager.Instance.BattleStateUI();
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

    //Collider를 지닌 오브젝트를 탐지함, 추후에 combat에서 사용하기 위해 분리함
    public GameObject selectCollider()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        // hit한 collider가 있는지 확인
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
    
        return null;
    }
    
    // 버튼에 들어갈 함수
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
