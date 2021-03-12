using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public bool displayGridGizmos = false;
    public Vector2 gridWorldSize; //Area of the grid
    public float nodeRadius; //How much space an individual node covers
    public LayerMask unwalkableMask; //Nodes that are unwalkable
    public TerrainType[] walkableRegions;
    public int obstacleProximityPenalty = 10;

    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    LayerMask walkableMask; //Layers that are walkable
    AStarNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;
    //public int blurSize;
    // Start is called before the first frame update
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value; //Bitwise stuff
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        CreateGrid();
    }

    public void ClearMapDictionary()
    {
        if(walkableRegionsDictionary != null)
        {
            walkableRegionsDictionary.Clear();
        }
        else
        {
            Debug.Log("Dictionary is already empty.");
        }
    }

    public void RegenerateMapDictionary()
    {
        if(walkableRegionsDictionary.Count <= 0)
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            foreach (TerrainType region in walkableRegions)
            {
                walkableMask.value |= region.terrainMask.value; //Bitwise stuff
                walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
            }
            CreateGrid();
        } 
        else
        {
            Debug.Log("Keys already exist, please clear map first!");
        }
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
                int movementPenalty = 0;

                //raycast that compares movementPenalty

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                grid[x, y] = new AStarNode(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
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

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;// kernel size must be odd numbers
        int kernelExtents = (kernelSize - 1) / 2;// The limit of the kernel, how many squares are between the central square and the edge of the kernel

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++) //For the first node we need to loop through and sum them up, only the following nodes get to use the formula of: (PreviousSum - 1) - (NextIndex - 1) + (NextIndex + 1)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                //2D array at position 0, y stores the movement penalty
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX); //index of the node that is no longer inside the kernel as it shifts over
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1); //node that has just entered the kernel

                //The horizontal value at location x,y is assigned (incremented): 
                //the previous sum at location [x - 1, y] minus movement penalty value of the previous node grid[removeIndex,y] offset/added by the movement penalty value of the next node grid[addIndex,y]
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++) //For the first node we need to loop through and sum them up, only the following nodes get to use the formula of: (PreviousSum - 1) - (NextIndex - 1) + (NextIndex + 1)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY]; //Uses the previous horizontal sums found above
            }

            //Blurs the first row
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY); //index of the node that is no longer inside the kernel as it shifts over
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1); //node that has just entered the kernel

                //The horizontal value at location x,y is assigned (incremented): 
                //the previous sum at location [x - 1, y] minus movement penalty value of the previous node grid[removeIndex,y] offset/added by the movement penalty value of the next node grid[addIndex,y]
                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }
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
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red; //Color is set to white or red if the node is walkable  
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
            }
        }
    }
}

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}
