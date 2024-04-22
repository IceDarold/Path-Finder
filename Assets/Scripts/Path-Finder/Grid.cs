using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] grid;
    [Range(0, 10)]
    public float nodeRadius;
    public LayerMask unwalkableMask;
    int gridSizeX, gridSizeY;
    [HideInInspector]
    public float nodeDiameter;
    public bool recalculate;
    [Header("Visualization")]
    [SerializeField] private bool drawGrid;
    private void OnValidate()
    {
        transform.localScale = new Vector3(
            transform.localScale.x >= 0 ? transform.localScale.x : 0,
            transform.localScale.y >= 0 ? transform.localScale.y : 0,
            transform.localScale.z >= 0 ? transform.localScale.z : 0);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(transform.localScale.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(transform.localScale.z / nodeDiameter);
    }

    private void Update()
    {
        if (recalculate)
        {
            recalculate = false;
            CreateGrid();
        }
    }
    void Awake()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 gridLefBottomPos = transform.position - Vector3.right * transform.localScale.x / 2 - Vector3.forward * transform.localScale.z / 2;
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector3 nodePos = gridLefBottomPos + Vector3.right * (x * nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(nodePos, nodeRadius, unwalkableMask);
                Node newNode = new Node(walkable, nodePos);
                grid[x, y] = newNode;
                if (x > 0)
                {
                    newNode.neighbours[0] = grid[x - 1, y];
                    grid[x - 1, y].neighbours[2] = newNode;
                }
                if (y > 0)
                {
                    newNode.neighbours[1] = grid[x, y - 1];
                    grid[x, y - 1].neighbours[3] = newNode;
                }
            }
        } 
    }
    public Node GetNodeByWorldPosition(Vector3 position)
    {
        float percentX = (position.x - transform.position.x + transform.localScale.x / 2) / transform.localScale.x;
        float percentY = (position.z - transform.position.z + transform.localScale.z / 2) / transform.localScale.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        return grid[Mathf.RoundToInt(percentX * gridSizeX), Mathf.RoundToInt(percentY * gridSizeY)];
    }

    private void OnDrawGizmos()
    {
        if (!drawGrid) return;
        Gizmos.DrawWireCube(transform.position, Vector3.right * gridSizeX * nodeDiameter + Vector3.forward * gridSizeY * nodeDiameter);
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.green : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * nodeDiameter * 0.9f + Vector3.up * 0.1F);
            }
        }
    }
} 