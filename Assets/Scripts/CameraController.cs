using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCameraController : MonoBehaviour
{
    Transform player;
    bool DebugMode = false;
    [SerializeField] float smoothing = 0.2f;
    [SerializeField] Vector2 minCameraBoundary;
    [SerializeField] Vector2 maxCameraBoundary;
    public Vector3 moveDirection = Vector3.zero;
    public float moveSpeed = 1f;
    
    private void Start()
    {
        player = UnitManager.Instance.allyUnits[0].gameObject.transform;
        if (GameManager.Instance.gameState == GameState.Debug) DebugMode = true;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Battle && !DebugMode)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);

            targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }else if (DebugMode)
        {
            transform.position += moveDirection * moveSpeed;
        }
    }
    
    /*public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        
        
    }*/
}
