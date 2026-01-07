using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public TileMapManager map;
    public int tileX,tileY; // cell 좌표임
    public List<Node> currentPath = null;

    void Update(){
        if(currentPath != null){
            int currNode = 0;
            // print("설마");
            while(currNode < currentPath.Count-1){
                Vector3 start = map.CellCoordToWorldCoord(currentPath[currNode].x,currentPath[currNode].y);
                Vector3 end = map.CellCoordToWorldCoord(currentPath[currNode+1].x ,currentPath[currNode+1].y);

                Debug.DrawLine(start,end,Color.red);
                currNode++;
            }
        }
    }
}
