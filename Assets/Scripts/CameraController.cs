using System;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCameraController : MonoBehaviour
{
    Transform player;
    bool DebugMode = false;
    [SerializeField] float smoothing = 0.2f;
    [SerializeField] GameObject minCameraBoundaryObject;
    [SerializeField] GameObject maxCameraBoundaryObject;
    Vector2 minCameraBoundary;
    Vector2 maxCameraBoundary;
    public Vector3 moveDirection = Vector3.zero;
    public float moveSpeed = 0.1f;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChange += OnDebugMod;
        
        minCameraBoundary = minCameraBoundaryObject.transform.position;
        maxCameraBoundary = maxCameraBoundaryObject.transform.position;
        minCameraBoundaryObject.SetActive(false);
        maxCameraBoundaryObject.SetActive(false);
    }

    private void Start()
    {
        if(GameManager.Instance.gameState == GameState.Battle)
            player = UnitManager.Instance.allyUnits[0].gameObject.transform;
    }

    void OnDebugMod()
    {
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
        }
        else if (DebugMode)
        {
            transform.position += moveDirection * moveSpeed;
        }
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }else if (context.canceled)
        {
            moveDirection = Vector3.zero;
        }
    }
}
