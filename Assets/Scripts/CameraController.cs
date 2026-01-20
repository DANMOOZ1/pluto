using System;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    Transform player;
    [SerializeField] float smoothing = 0.2f;
    [SerializeField] Vector2 minCameraBoundary;
    [SerializeField] Vector2 maxCameraBoundary;

    private void Start()
    {
        player = UnitManager.Instance.accessibleUnits[0].gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Battle)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);

            targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }
        else if (GameManager.Instance.gameState == GameState.Debug)
        {
            transform.position = new Vector3((maxCameraBoundary.x + minCameraBoundary.x)/2, (maxCameraBoundary.y + minCameraBoundary.y)/2, this.transform.position.z);
            //gameObject.GetComponent<Camera>().size
        }
        
    }
}
