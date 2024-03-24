using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public Node[,] grid;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    int gridSizeX, gridSizeY;
    float nodeDiameter;
    private void OnValidate()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
    }

    void Start()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 gridLefBottomPos = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePos = gridLefBottomPos + Vector3.right * (x * nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(nodePos, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, nodePos);
            }
        }
    }
    public Node GetNodeByWorldPosition(Vector3 position)
    {
        float percentX = (position.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (position.z - transform.position.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        return grid[Mathf.RoundToInt(percentX * gridSizeX), Mathf.RoundToInt(percentY * gridSizeY)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.right * gridSizeX * nodeDiameter + Vector3.forward * gridSizeY * nodeDiameter);
        if (grid != null)
        {
            foreach (Node node in grid) 
            {
                Gizmos.color = node.walkable ? Color.green : Color.red;
                if (node == GetNodeByWorldPosition(player.position))
                {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * nodeDiameter * 0.9f + Vector3.up * 0.1F);
            }   
        }
    }
}
