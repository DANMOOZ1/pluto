using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
public class InputHandler : MonoBehaviour
{

    private Camera _mainCamera;
    public TileMapManager map;
    public Tilemap tilemap;

    private void Awake(){
        _mainCamera = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context){
        if(!context.started) return;
        
        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePosTranslated = tilemap.WorldToCell(mousePos);
        map.GeneratePathTo(mousePosTranslated[0],mousePosTranslated[1]);
        // var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        // if (!rayHit.collider) return;

        // TilePrefabManager tilePrefabManager = rayHit.collider.gameObject.GetComponent<TilePrefabManager>();
        // tilePrefabManager.Clicked();
    }


}
