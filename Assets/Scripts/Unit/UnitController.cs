using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    public Vector3Int cellPosition; // cell 좌표임
    public List<Node> currentPath = null;
    
    // 이동 관련 변수 추가
    public float moveSpeed = 5f;  // 이동 속도
    private int currentPathIndex = 0;  // 현재 목표 노드 인덱스
    private bool isMoving = false;  // 이동 중인지 확인

    void Update()
    {
        // 경로 시각화 (디버그용)
        // if(currentPath != null)
        // {
        //     int currNode = 0;
            
        //     while(currNode < currentPath.Count - 1)
        //     {
        //         Vector3 start = map.CellCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
        //         Vector3 end = map.CellCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);

        //         Debug.DrawLine(start, end, Color.red);
        //         currNode++;
        //     }
        // }
        
        // 이동 처리
        if(isMoving && currentPath != null)
        {
            MoveAlongPath();
        }
    }
    
    // 경로 이동 시작
    public void StartMoving(List<Node> path)
    {
        currentPath = path;
        if(currentPath != null && currentPath.Count > 0)
        {
            currentPathIndex = 0;
            isMoving = true;
        }
    }
    
    // 경로를 따라 이동
    void MoveAlongPath()
    {
        if(currentPathIndex >= currentPath.Count)
        {
            // 이동 완료
            isMoving = false;
            currentPath = null;
            return;
        }
        
        Vector3Int targetCellPos = new Vector3Int(currentPath[currentPathIndex].x,currentPath[currentPathIndex].y,currentPath[currentPathIndex].z);
        // 목표 위치 계산
        Vector3 targetPosition = TileMapManager.Instance.CellCoordToWorldCoord(targetCellPos);
        
        // 현재 위치에서 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // 목표 지점 도착 확인
        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // 정확한 위치로 스냅
            transform.position = targetPosition;
            
            // 타일 좌표 업데이트
            cellPosition = targetCellPos;
            
            // 다음 노드로 이동
            currentPathIndex++;
        }
    }
}
