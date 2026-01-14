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
        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        int i = 0;
        Vector3Int pos = new Vector3Int();
        bool flag = false;
        foreach (var tilemap in TileMapManager.Instance.tilemaps)
        {
            Vector3Int mousePosTranslated = tilemap.WorldToCell(mousePos);
            mousePosTranslated = mousePosTranslated - new Vector3Int(5, 5, 0);// 왜인진 모르겠는데 isometric z as y 로 설정하면 x,y cell pos가 +5 됨
            mousePosTranslated.z = i;
            if (TileMapManager.Instance.dataOnTiles.ContainsKey(mousePosTranslated))
            {
                pos = mousePosTranslated;
                flag = true;
            }
            i++;
        }

        if (flag)
        {
            // 유닛 매니저에서 selected unit을 불러와 이동시킴
            GameObject unit = UnitManager.Instance.selectedUnit;
            Vector3Int currPos = unit.GetComponent<UnitController>().cellPosition;
            List<Node> currentPath = TileMapManager.Instance.GeneratePathTo(currPos, pos);
            
            unit.GetComponent<UnitController>().StartMoving(currentPath);
        }
    }


}
