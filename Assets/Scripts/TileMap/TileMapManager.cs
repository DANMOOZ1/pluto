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

    public Vector3  CellCoordToWorldCoord(Vector3Int cellPos){
        int x = cellPos.x;
        int y = cellPos.y;
        int z = cellPos.z;
        return new Vector3(x - y , x/2f + y/2f + 0.5f + z,0);
    }

    public Dictionary<Vector3Int, TileType> dataOnTiles; // key : cell pos , value : TileType
    public Dictionary<Vector3Int, Node> cellPosGraph;// key : cell pos , value : Node

    public void GenerateTileData(){
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
        //3단계: cellPosGraph에서 타일위에 바로 타일이 있는 경우에는 해당 타일 제거
        List<Vector3Int> blockedNodes = new List<Vector3Int>();
    
        foreach (Vector3Int v in cellPosGraph.Keys)
        {
            Vector3Int upperPos = new Vector3Int(v.x, v.y, v.z + 1);
            if (dataOnTiles.ContainsKey(upperPos))
            {
                blockedNodes.Add(v);
            }
        }
    
        Debug.Log($"총 {blockedNodes.Count}개의 차단된 노드 제거");
    
        foreach (Vector3Int v in blockedNodes)
        {
            cellPosGraph.Remove(v);
        }
        
        // 4단계: 이웃 노드 연결
        Vector3Int[] xyDirections = new Vector3Int[]
        {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0)
        };
        
        Vector3Int[] zDirections = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 0)
        };
        
        foreach (Vector3Int v in cellPosGraph.Keys)
        {
            foreach (Vector3Int zDir in zDirections)
                foreach (Vector3Int xyDir in xyDirections)
                {
                    Vector3Int newV = v + xyDir + zDir;
                    if (cellPosGraph.ContainsKey(newV))
                    {
                        // 에스컬레이터 체크
                        if (zDir.z == 1)
                        {
                            if (dataOnTiles[newV].escalator == false) continue;
                        }
                        else if (zDir.z == -1)
                        {
                            if (dataOnTiles[v].escalator == false) continue;
                        }
                    
                        cellPosGraph[v].neighbours.Add(cellPosGraph[newV]);
                    }
                }
        }
    } 

    //입력 : cell 좌표계 기반으로 해당 타일의 이동 코스트를 출력
    private float CostToEnterTile(Vector3Int cellPos){
        return dataOnTiles[cellPos].movementCost; 
    }

    // cell 좌표를 입력 받아 해당 목적지까지의 경로를 작성함(unit 클래스 자체의 currentPath 에 접근하여 작성)
    public List<Node> GeneratePathTo(Vector3Int currPos,Vector3Int targetPos, TileCheckRule movementRule, bool ignoreUnit = false){
        if (!cellPosGraph.ContainsKey(targetPos)) {
            Debug.LogWarning($"목표 위치 {targetPos}에 타일이 없습니다.");
            return null;
        }
        
        if (!cellPosGraph.ContainsKey(currPos)) {
            Debug.LogWarning($"유닛 위치 {currPos}에 타일이 없습니다.");
            return null;
        }
        
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();
        
        Node source = cellPosGraph[currPos];
        Node target = cellPosGraph[targetPos];
        
        dist[source] = 0;
        prev[source] = null;
        
        foreach(Node v in cellPosGraph.Values)
        {
            if(v != null)
            {
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }
                unvisited.Add(v);
            }
        }
        
        while(unvisited.Count > 0)
        {
            Node u = null;
            foreach(Node possibleU in unvisited)
            {
                if(u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }
            
            if (dist[u] == Mathf.Infinity)
            {
                return null;
            }

            if (u == target)
            {
                break;
            }
            
            unvisited.Remove(u);

            // 핵심 변경: 행마법을 고려한 이웃 탐색
            foreach(Node v in u.neighbours)
            {
                Vector3Int fromPos = new Vector3Int(u.x, u.y, u.z);
                Vector3Int toPos = new Vector3Int(v.x, v.y, v.z);
                
                // 행마법 체크: 이 이동이 허용되는지 확인
                if (!movementRule.TileCheckRuleFunc(fromPos, toPos)) continue;
                // 유닛이 해당 이동 경로에 존재하는 지 확인, ignoreUnit이 True면 확인 안함
                if(!ignoreUnit && UnitManager.Instance.UnitOnTile(toPos)) continue;
                
                float alt = dist[u] + CostToEnterTile(toPos);
                if (Mathf.Abs(fromPos.x - toPos.x) + Mathf.Abs(fromPos.y - toPos.y) >= 2) alt += 0.000001f;
                if(alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        List<Node> currentPath = new List<Node>();
        Node curr = target;
        
        while(curr != null && prev[curr] != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        
        currentPath.Reverse();

        return currentPath;
    }

    public TileBase ReachableTilePrefab;
    public TileBase ReachableTilePrefab2;
    // 이동 가능한 타일을 RTPrefab으로 표현하고 이동가능한 타일의 셀좌표 keycollection을 반환함
    public List<Vector3Int> ReachableTile(Vector3Int pos, TileCheckRule movementRule)
    {
        //RNG에 해당하는 z좌표를 고려하지 않은 이동가능한 타일을 가져옴
        List<Vector3Int> dist = ReturnInteractiveTiles(pos, movementRule);
        List<Vector3Int> returnDist = new List<Vector3Int>();
        
        foreach (Vector3Int v in dist)
        {
            List<Node> path = GeneratePathTo(pos, v, movementRule);
            if (path != null)
            {
                if (movementRule.teleportTypeMovement) // 나이트 처럼 텔레포팅하면서 움직이는 경우
                {
                    returnDist.Add(v);
                    continue;
                }
                
                //RNG안에서 걸어서 이동하는 경우
                bool isPathValid = true;
                foreach (Node n in path)
                {
                    if (!dist.Contains(new Vector3Int(n.x, n.y, n.z)))
                    {
                        isPathValid = false;
                        break;
                    }
                }

                if (isPathValid)
                {
                    returnDist.Add(v);
                }
            }
        }

        DrawTile(ReachableTilePrefab,ReachableTilePrefab2,returnDist);
        
        return returnDist;
        
    }
    
    public TileBase AttackableTilePrefab;
    public TileBase AttackableTilePrefab2;

    public List<Vector3Int> AttackableTile(Vector3Int pos, TileCheckRule atkRule)
    {
        List<Vector3Int> dist = ReturnInteractiveTiles(pos, atkRule);

        DrawTile(AttackableTilePrefab,AttackableTilePrefab2,dist);
        
        return dist;
    }

    public List<Vector3Int> ReturnInteractiveTiles(Vector3Int pos, TileCheckRule Rule)
    {
        List<Vector3Int> dist = new List<Vector3Int>();

        foreach (Vector3Int v in cellPosGraph.Keys)
        {
            if (Rule.TileCheckRuleFunc(v, pos)) dist.Add(v);
        }
        
        return dist;
    }
    public void DrawTile(TileBase tile1, TileBase tile2, List<Vector3Int> positions)
    {
        foreach (Vector3Int v in positions)
        {
            Vector3Int p = new Vector3Int(v[0], v[1], 0);

            if (dataOnTiles[v].escalator) subTilemaps[v[2]].SetTile(p, tile2);
            else subTilemaps[v[2]].SetTile(p, tile1);
        }
    }

    public void ClearTileMap(IEnumerable<Vector3Int> keys)
    {
        foreach (Vector3Int v in keys)
        {
            Vector3Int p  = new Vector3Int(v[0],v[1],0);
            subTilemaps[v[2]].SetTile(p, null);
        }
    }
}
