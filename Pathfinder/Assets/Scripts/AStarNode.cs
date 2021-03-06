using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode>
{
    public bool walkable; //Is the node traversable or walkable?
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public AStarNode parent;
    //Constructor assigns values when creating nodes
    public AStarNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex; 
        }
        set
        {
            heapIndex = value;
        }
    }


    public int CompareTo(AStarNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) //If the fcost is equal
        {
            compare = hCost.CompareTo(nodeToCompare.hCost); //Uses the h cost
        }
        return -compare;
    }
}
