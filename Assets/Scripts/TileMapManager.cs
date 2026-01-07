using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;

public class TileMapManager : MonoBehaviour
{
    //타일맵을 불러옴
    public Tilemap tilemap; 
    //인스펙터에서 타일 종류를 다루기 위한 직렬화된 딕셔너리
    public SerializedDictionary<string, TileType> tileTypes;
    //현재 선택된 유닛(추후 개발)
    public GameObject selectedUnit;
    //셀 위치에 타일 정보를 저장할 행렬

    int width;
    int height;

    void Start(){
        //유닛의 시작 위치 초기화
        selectedUnit.GetComponent<UnitManager>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<UnitManager>().tileY = (int)selectedUnit.transform.position.y;
        selectedUnit.GetComponent<UnitManager>().map = this;

        GenerateTileData();
    }
    public int GetAxisRange(List<Vector3Int> keys, string axis)
    {
        int max = int.MinValue;
        int min = int.MaxValue;

        foreach (Vector3Int key in keys)
        {
            int value = axis.ToLower() switch
            {
                "x" => key.x,
                "y" => key.y,
                "z" => key.z,
                _ => key.x
            };

            if (value > max) max = value;
            if (value < min) min = value;
        }

        return max - min;
    }

    public Vector3  CellCoordToWorldCoord(int x, int y){
        return new Vector3(x + 0.5f, y + 0.5f,-1);
    }

    public Dictionary<Vector3Int, TileType> dataOnTiles = new Dictionary<Vector3Int, TileType>();// key : cell pos , value : TileType
    public Dictionary<Vector3Int, Node> cellPosGraph = new Dictionary<Vector3Int, Node>();// key : cell pos , value : Node

    void GenerateTileData(){
        // 타일 맵 생성(2차원 배열)
        // 타일맵 내의 셀 좌표들에 대해서 타일이 있다면 딕셔너리에 초기 정보를 추가한다. pos는 셀 좌표
        //출처: https://upbo.tistory.com/111 [메모장:티스토리]
        foreach(Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            // 해당 좌표에 타일이 없으면 넘어간다.
            if(!tilemap.HasTile(pos)) continue;
            // 해당 좌표의 타일을 얻는다.
            var tile = tilemap.GetTile<TileBase>(pos);

            // 정보 초기화, 타일 이름이 종류를 분간함, 인스펙터에서 설정한 데이터는 tileTypes에서 이름 넣어서 불러오면 됨
            dataOnTiles[pos] = new TileType();
            dataOnTiles[pos].name = tile.name;
            dataOnTiles[pos].tileX = pos[0];
            dataOnTiles[pos].tileY = pos[1];
            dataOnTiles[pos].movementCost = tileTypes[tile.name].movementCost;
        }
        
        //셀 좌표를 이용해서 graph에 쓸 width와 height 구하기 
        width = GetAxisRange(dataOnTiles.Keys.ToList(), "x");
        height = GetAxisRange(dataOnTiles.Keys.ToList(), "y");

        //cellPosGraph 작성 및 neighbours 추가
        // 1. 모든 노드 생성
        foreach(Vector3Int v in dataOnTiles.Keys)
        {
            cellPosGraph[v] = new Node { x = v[0], y = v[1] }; // 객체 초기화 구문
        }
        
        // 2. 이웃 노드 연결 (4방향)
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(-1, 0, 0),  // 왼쪽
            new Vector3Int(1, 0, 0),   // 오른쪽
            new Vector3Int(0, -1, 0),  // 아래
            new Vector3Int(0, 1, 0)    // 위
        };
        
        foreach(Vector3Int v in dataOnTiles.Keys)
        {            
            foreach(Vector3Int dir in directions)
            {
                Vector3Int newV = v + dir;
                
                // 범위 체크 및 노드 존재 확인
                if (cellPosGraph.ContainsKey(newV))
                {
                    cellPosGraph[v].neighbours.Add(cellPosGraph[newV]);
                }
            }
        }
        
        // foreach(Node t in graph){
        //     if (t != null){
        //         int i = 0;
        //         print(t.x+","+t.y);
        //         foreach(Node j in t.neighbours){
        //             i++;
        //         }
        //         print(i);
        //     }
            
        // }
        
        
    }

    //입력 : cell 좌표계 기반으로 해당 타일의 이동 코스트를 출력
    private float CostToEnterTile(int x, int y){
        Vector3Int pos = new Vector3Int(x, y ,0);       
        return dataOnTiles[pos].movementCost; 
    }

    // cell 좌표를 입력 받아 해당 목적지까지의 경로를 작성함(unit 클래스 자체의 currentPath 에 접근하여 작성)
    public void GeneratePathTo(int tileX, int tileY){
        // selectedUnit.transform.position = TileCoordToWorldCoord(x,y);
        //길찾기는 다익스트라 알고리즘을 활용하여 이루어짐

        int x = tileX;
        int y = tileY;
        Vector3Int pos = new Vector3Int(x,y,0);

        //타겟이 타일 맵 밖일때
        if (x < 0 || x >= width) return;
        if (y < 0 || y >= height) return;
        //선택된 유닛의 기존 경로 초기화
        selectedUnit.GetComponent<UnitManager>().currentPath = null;
        
        
        Dictionary<Node,float> dist = new Dictionary<Node, float>();
        Dictionary<Node,Node> prev = new Dictionary<Node, Node>();
        // 아직 방문하지 않은 노드의 리스트
        List<Node> unvisited = new List<Node>();
        //시작점, 도착점(selected unit을 이용)
        Node source = cellPosGraph[new Vector3Int(selectedUnit.GetComponent<UnitManager>().tileX, selectedUnit.GetComponent<UnitManager>().tileY, 0)];
        Node target = cellPosGraph[pos];
        //시작점에서의 초기값 설정, dist는 시작점에서 목표점 까지의 거리, prev는 현재 까지 진행된 경로의 체인
        dist[source] = 0;
        prev[source] = null;
        
        // 모든 타일 사이의 거리를 무한대로 초기화
        foreach(Node v in cellPosGraph.Values){
            if(v != null){
                if (v != source){
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }
                unvisited.Add(v);
            }
            
        }
        
        while(unvisited.Count > 0){
            // 느리지만 코드가 간단해용, 시간남으면 우선순위 큐로 최적화 하삼
            // u : 방문하지 않은 노드중 가장 짧은 거리에 있는 노드
            Node u = null;
            foreach(Node possibleU in unvisited){
                if(u == null || dist[possibleU] < dist[u]){
                    u = possibleU;
                }
            }

            if (u == target){
                break; // while 루프 탈출(타켓의 최소 경로 탐색 완료)
            }
            unvisited.Remove(u);

            foreach(Node v in u.neighbours){
                // float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(v.x,v.y);
                if(alt < dist[v]){
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
        // 경로가 존재 X
        if(prev[target] == null){
            return;
        }

        List<Node> currentPath = new List<Node>();

        Node curr = target;
        //prev 의 시퀀스 체인을 따라가며 추가함
        while(prev[curr] != null){
            currentPath.Add(curr);
            curr = prev[curr];
        }
        
        currentPath.Reverse();
        Debug.Log(dist[target]);

        selectedUnit.GetComponent<UnitManager>().currentPath = currentPath;

    }

}
