using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public bool displayGridGizmos = false;
    public LayerMask unwalkableMask; //Nodes that are unwalkable
    public Vector2 gridWorldSize; //Area of the grid
    public float nodeRadius; //How much space an individual node covers
    AStarNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // Start is called before the first frame update
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize { get => gridSizeX * gridSizeY; }
    //Creates the 2D Grid
    void CreateGrid()
    {
        grid = new AStarNode[gridSizeX, gridSizeY];

        //Uses the bottom left of the world as reference
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //This gives us the left edge of the world and bottom left corner
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new AStarNode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<AStarNode> GetNeighbors(AStarNode node)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        for (int x = -1; x <= 1; x++)
        {
            //If this is relative to the node's position then we're in the center of the 3x3 block (because it is the center of that block due to being the given node)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 & y == 0)
                    continue; //Skips this particular iteration

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public AStarNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; //How far along the grid it is (far left 0, middle percentage of 5 and far right is 1)
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; //world position is z here because of the way the screen is oriented

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY); //Ensures numbers will be between 0 and 1

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y)); //This is because our y axis is the Z axis 
        if (grid != null && displayGridGizmos)
        {
            foreach (AStarNode n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red; //Color is set to white or red if the node is walkable  
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
