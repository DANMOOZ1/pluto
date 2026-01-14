using UnityEngine;

[System.Serializable]
public class TileType
{
    public Vector3Int cellPosition;

    public string name; 
    public float movementCost = 1;
    public bool escalator =  false;
}
