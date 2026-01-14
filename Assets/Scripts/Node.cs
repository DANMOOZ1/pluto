using UnityEngine;
using System.Collections.Generic;

public class Node {
    public List<Node> neighbours;
    public int x,y,z;

    public Node(){
        neighbours = new List<Node>();
    }
}
