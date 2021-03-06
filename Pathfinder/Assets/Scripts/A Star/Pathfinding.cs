﻿using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    //The Path manager will handle this now
    //  public Transform seeker, target;

    AStarGrid grid;
    public float timeToDestination;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        AStarNode startNode = grid.NodeFromWorldPoint(request.pathStart);
        AStarNode targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        if (startNode.walkable && targetNode.walkable)
        {
            //List of nodes to be evaluated
            Heap<AStarNode> openSet = new Heap<AStarNode>(grid.MaxSize);
            //List of closed nodes that have been evaluated
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
            openSet.Add(startNode); //Start by evaluating from the start node

            while (openSet.Count > 0)
            {
                AStarNode currentNode = openSet.RemoveFirstItem(); //The previous loops completed in one line (technically)
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    timeToDestination = sw.ElapsedMilliseconds;
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break; ; //Path found
                }

                foreach (AStarNode neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue; //Skips this neighbor if evaluated or not walkable
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                        {
                            //If it's already in the open set then just update the value
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }
    }

    //Once we've found the target node we need to retrace the steps to get the path to the start node to the end node
    Vector3[] RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    //This simplifies the paths so the waypoints are ONLY placed where the path changes directions
    Vector3[] SimplifyPath(List<AStarNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            //Direction between last two nodes
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    //Get distance between node A and B
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
