using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;

//isometric 맵 제작하는 법!
// 1. grid에 Layer (number) 로 isometric tilemap을 생성한다.
// 2. tilemap의 transform.y 를 (number)로 설정한다.
// 3. order in layer을 (number)로 설정한다.
// 4. TileMapManager의 tilemap 리스트에 만든 tilemap 게임오브젝트를 추가한다.(순서 중요!!!)

public class TileMapManager : Singleton<TileMapManager>
{
    //타일맵을 불러옴
    public Tilemap[] tilemaps; 
    public Tilemap[] subTilemaps;
    //인스펙터에서 타일 종류를 다루기 위한 직렬화된 딕셔너리
    public SerializedDictionary<string, TileType> tileTypes;
    //현재 선택된 유닛(추후 개발)
    
    void Start(){

        GenerateTileData();
    }

    public Vector3  CellCoordToWorldCoord(Vector3Int cellPos){
        int x = cellPos.x;
        int y = cellPos.y;
        int z = cellPos.z;
        return new Vector3(x - y , x/2f + y/2f + 0.5f + z,0);
    }

    public Dictionary<Vector3Int, TileType> dataOnTiles; // key : cell pos , value : TileType
    public Dictionary<Vector3Int, Node> cellPosGraph;// key : cell pos , value : Node

    void GenerateTileData(){
        // 0단계 데이터 초기화
        dataOnTiles = new Dictionary<Vector3Int, TileType>();
        cellPosGraph = new Dictionary<Vector3Int, Node>();
        // 1단계: 모든 타일맵에서 타일 데이터 수집
        int z = 0;
        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos)) continue;
                var tile = tilemap.GetTile<TileBase>(pos);
                Vector3Int realPos = pos;
                realPos.z = z;
                // 중복 체크 추가 (필요시)
                if (!dataOnTiles.ContainsKey(realPos))
                {
                    dataOnTiles[realPos] = new TileType();
                    dataOnTiles[realPos].name = tile.name;
                    dataOnTiles[realPos].cellPosition = realPos;
                    dataOnTiles[realPos].movementCost = tileTypes[tile.name].movementCost;
                    dataOnTiles[realPos].escalator = tileTypes[tile.name].escalator;
                }
            }
                
            z++;
        }

        // 2단계: 그래프 생성 (타일맵 반복문 밖으로 이동)
        foreach (Vector3Int v in dataOnTiles.Keys)
        {
            cellPosGraph[v] = new Node { x = v[0], y = v[1], z = v[2] };
        }

        // 3단계: 이웃 노드 연결
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
            
        };

        foreach (Vector3Int v in dataOnTiles.Keys)
        {
            foreach (Vector3Int dir in directions)
            {
                Vector3Int newV = v + dir;
                if (cellPosGraph.ContainsKey(newV))
                {
                    if (dir.z == 1)//올라갈때 도착 타일이 에스컬레이터 면
                    {
                        if (dataOnTiles[newV].escalator == false) continue;
                    }else if (dir.z == -1)//내려갈때 출발 타일이 에스컬레이터 면
                    {
                        if (dataOnTiles[v].escalator == false) continue;
                    }
                    cellPosGraph[v].neighbours.Add(cellPosGraph[newV]);
                }
            }
        }

        // 모든 타일 보는 코드(디버깅 용)
        /*foreach (Vector3Int v in cellPosGraph.Keys)
        {
            print("main node:"+v);
            foreach (Node n in cellPosGraph[v].neighbours)
            {
                print(n.x+" "+n.y+" "+n.z);
            }
        }*/
    } 

    //입력 : cell 좌표계 기반으로 해당 타일의 이동 코스트를 출력
    private float CostToEnterTile(Vector3Int cellPos){
        return dataOnTiles[cellPos].movementCost; 
    }

    // cell 좌표를 입력 받아 해당 목적지까지의 경로를 작성함(unit 클래스 자체의 currentPath 에 접근하여 작성)
    public List<Node> GeneratePathTo(Vector3Int currPos,Vector3Int targetPos){
        // selected unit Null 체크
        
        if (!cellPosGraph.ContainsKey(targetPos)) {
            Debug.LogWarning($"목표 위치 {targetPos}에 타일이 없습니다.");
            return null;
        }
        
        Dictionary<Node,float> dist = new Dictionary<Node, float>();
        Dictionary<Node,Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();
        
        // target tile Null 체크
        if (!cellPosGraph.ContainsKey(currPos)) {
            Debug.LogWarning($"유닛 위치 {currPos}에 타일이 없습니다.");
            return null;
        }
        
        Node source = cellPosGraph[currPos];
        Node target = cellPosGraph[targetPos];
        
        dist[source] = 0;
        prev[source] = null;
        
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
            Node u = null;
            foreach(Node possibleU in unvisited){
                if(u == null || dist[possibleU] < dist[u]){
                    u = possibleU;
                }
            }
            
            if (dist[u] == Mathf.Infinity){
                break;
            }

            if (u == target){
                break;
            }
            
            unvisited.Remove(u);

            foreach(Node v in u.neighbours){
                float alt = dist[u] + CostToEnterTile(new Vector3Int(v.x,v.y,v.z));
                if(alt < dist[v]){
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        List<Node> currentPath = new List<Node>();
        Node curr = target;
        
        while(curr != null && prev[curr] != null){
            currentPath.Add(curr);
            curr = prev[curr];
        }
        
        currentPath.Reverse();

        return currentPath;
    }

    public TileBase RTPrefab;
    public void ReachableTile(Vector3Int pos, int mov)
    {
        Dictionary<Vector3Int,List<Node>> dist = new Dictionary<Vector3Int,List<Node>> ();
        
        foreach (Vector3Int v in dataOnTiles.Keys)
        {
            dist[v] = GeneratePathTo(pos, v);
            if (dist[v] == null || dist[v].Count > mov)
            {
                dist.Remove(v);
            }
        }
        
        foreach (Vector3Int v in dist.Keys)
        {
            Vector3Int p  = new Vector3Int(v[0],v[1],0);
            subTilemaps[v[2]].SetTile(p, RTPrefab);
        }
        
    }
}
