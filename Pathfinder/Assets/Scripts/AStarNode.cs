using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public bool walkable; //Is the node traversable or walkable?
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public AStarNode parent;

    //Constructor assigns values when creating nodes
    public AStarNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; } 
    }    
}
