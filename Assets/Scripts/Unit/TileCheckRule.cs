using System.Collections.Generic;
using UnityEngine;

public abstract class TileCheckRule : ScriptableObject
{
    public bool teleportTypeMovement;
    public int mov;
    public abstract bool TileCheckRuleFunc(Vector3Int from, Vector3Int to, List<Node> path = null);
}
