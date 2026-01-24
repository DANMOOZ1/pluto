using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{

    private Camera _mainCamera;
    private void Awake(){
        _mainCamera = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context){
        if(!context.started) return;
        
        if(GameManager.Instance.gameState == GameState.Battle && UnitManager.Instance.selectedUnit.isAlly)
            switch (GameManager.Instance.battleState)
            {
                case BattleState.Default:
                    InputAnalyze();
                    break;
                case BattleState.Move:
                    if (!UnitManager.Instance.selectedUnit.isMoving) InputAnalyze();
                    break;
                case BattleState.Combat:
                    UnitAttack();
                    break;
                case BattleState.Next:
                    break;
                case BattleState.Info:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
    // 마우스 위치에 선택된 유닛을 소환함
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(!context.started) return;

        if (GameManager.Instance.gameState == GameState.Debug)
        {
            Vector3Int? mousePos = MousePosToCellPos();


            if (mousePos.HasValue)
            {
                DataManager.Instance.AddUnit(mousePos.Value);
            }
        }
    }

    //Debug 모드에서 StageData를 json에 저장시킴
    public void PressSKey(InputAction.CallbackContext context)
    {
        if(!context.started) return;

        if (GameManager.Instance.gameState == GameState.Debug)
        {
            DataManager.Instance.SaveStageData();
        }
    }

    //StageData에서 감지된 유닛을 제거하고 씬에서 감지된 오브젝트를 제거함
    public void PressDeleteKey(InputAction.CallbackContext context)
    {
        if(!context.started) return;

        if (GameManager.Instance.gameState == GameState.Debug)
        {
            GameObject obj = selectCollider();
            
            if (obj != null) DataManager.Instance.DeleteUnit(obj);
            else Debug.Log("마우스 위치에 유닛이 존재하지 않습니다.");
        }
    }

    //Page Down, Page Up으로 waveIndex를 조절함
    public void ControlWaveIndex(InputAction.CallbackContext context)
    {
        if(!context.started) return;
        
        if (GameManager.Instance.gameState == GameState.Debug)
            DataManager.Instance.ChangeWaveIndex((int)context.ReadValue<float>());
    }
    //------------------------------------------------------<Input 이외 함수>---------------------------------------------------------
    
    //마우스 위치에 해당하는 cellpos를 출력, 해당하는 타일이 없으면 null 출력
    public Vector3Int? MousePosToCellPos()
    {
        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        Vector3Int? cellPos = WorldPosToCellPos(mousePos); 
        Vector3Int? cellPos2 = WorldPosToCellPos(mousePos + new Vector3(0, 0.5f, 0));
        
        //반블록 인식을 위한 재검증
        if (!cellPos2.HasValue) return cellPos;
        if(TileMapManager.Instance.dataOnTiles[(Vector3Int)cellPos2].escalator) return cellPos2;
        return cellPos;
    }

    //입력에 해당하는 worldpos를 위치에 해당하는 가장 높은 cellpos로 변환해줌
    public Vector3Int? WorldPosToCellPos(Vector3 worldPos)
    {
        int i = 0;
        Vector3Int? foundPos = null;
    
        foreach (var tilemap in TileMapManager.Instance.tilemaps)
        {
            Vector3Int worldPosTranslated = tilemap.WorldToCell(worldPos);
            worldPosTranslated = worldPosTranslated - new Vector3Int(5, 5, 0); // 왜인진 모르겠는데 isometric z as y 로 설정하면 x,y cell pos가 +5 됨
            worldPosTranslated.z = i;
        
            if (TileMapManager.Instance.dataOnTiles.ContainsKey(worldPosTranslated))
            {
                foundPos = worldPosTranslated;
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
        // 이거 여기에 넣어도 괜찮을까요?? 다른 방법도 있어서 안 괜찮으면 말씀해주세용
        UnitManager.Instance.selectedAlly = null;
        UnitManager.Instance.selectedEnemy = null;
        
        // 선택된 오브젝트가 없으면 이동
        if (obj == null)
        {
            if (GameManager.Instance.battleState == BattleState.Move) // (+제가 default state일 때도 써야 해서 Move State 일 때 추가했습니다!)
            {
                UnitMove();
            }

            // UI 수정
            UIManager.Instance.BattleStateUI();
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
        }
        else // 제가 아군도 필요하게 되어 버렸어요..
        {
            UnitManager.Instance.selectedAlly = unit;
        }

        // UI 수정
        UIManager.Instance.BattleStateUI();
    }
    public void UnitMove()
    {
        Vector3Int? cellpos = MousePosToCellPos();

        if (!cellpos.HasValue)
        {
            print("마우스 위치에 타일이 존재하지 않습니다.");
            return;
        }
        UnitManager.Instance.selectedUnit.StartMoving(cellpos.Value);
    }

    public void UnitAttack()
    {
        GameObject obj = selectCollider();
        
        //유닛 선택이 안된경우 return
        if (obj == null) return;
            
        Unit unit = obj.GetComponent<Unit>();
        
        //두 선택 유닛간 진영이 다를 경우 Attack!
        if (unit.isAlly != UnitManager.Instance.selectedUnit.isAlly) UnitManager.Instance.selectedUnit.Attack(unit);
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

    public void DefaultButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Default);

    }

    public void IsAllyToggle(bool isOn)
    {
        if (isOn)
        {
            DataManager.Instance.isAlly = true;
            Debug.Log("ally");
        }
        else
        {
            DataManager.Instance.isAlly = false;
            Debug.Log("enemy");

        }
    }
}
