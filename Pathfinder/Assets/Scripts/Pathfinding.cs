using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    AStarGrid grid;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarNode startNode = grid.NodeFromWorldPoint(startPos);
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        //List of nodes to be evaluated
        List<AStarNode> openSet = new List<AStarNode>();

        //List of closed nodes that have been evaluated
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
        openSet.Add(startNode); //Start by evaluating from the start node

        //So long as the amount of nodes to be evaluated is greater than 0 then this loop shall go on
        while (openSet.Count > 0)
        {
            //Current node, whatever that may be, is given the value of the first element in the openset list
            AStarNode currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                //If the open set node at position i has a lower f cost then the current node, whatever that may be, then the current node is assigned that openset
                //Also if the openset's cost is equal to current node while the hcost is less than the current node's 
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
                }
            }

            //Current node has been evaluated, time to remove it to the openset and into the closedset
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return; //Path found
            }

            foreach (AStarNode neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue; //Skips this neighbor if evaluated or not walkable
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    //Once we've found the target node we need to retrace the steps to get the path to the start node to the end node
    void RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
