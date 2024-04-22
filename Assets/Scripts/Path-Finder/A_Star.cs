using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class A_Star : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Transform start;
    [SerializeField] private Transform goal;
    [SerializeField] private bool updatePathConstantly = true;
    [SerializeField] private bool update;
    [Header("Visualization")]
    [SerializeField] private bool drawPath = true;
    [SerializeField] private Color pathColor = Color.magenta;
    private List<Node> path;
    private Vector3 lastPlayerPos;
    private Vector3 lastGoalPos;
    private bool lastUpdateValue;

    private void Update()
    {
        if (start == null)
        {
            Debug.LogWarning("Assign field start!");
            return;
        }
        if (goal == null)
        {
            Debug.LogWarning("Assign field goal!");
            return;
        }
        if (updatePathConstantly && (Vector3.Distance(start.position, lastPlayerPos) > 1 || Vector3.Distance(goal.position, lastGoalPos) > 1))
        {
            RecalculatePath();
            lastPlayerPos = start.position;
            lastGoalPos = goal.position;
        }
        if (update != lastUpdateValue)
        {
            RecalculatePath();
            lastUpdateValue = update;
        }
    }
    private void Start()
    {
        //RecalculatePath();
    }
    private void RecalculatePath()
    {
        Node playerNode = grid.GetNodeByWorldPosition(start.position);
        Node goalNode = grid.GetNodeByWorldPosition(goal.position);
        path = GetPath(playerNode, goalNode);
    }
    private void OnDrawGizmos()
    {
        if (!drawPath) return;
        Gizmos.color = pathColor;
        if (path != null)
        {
            foreach (Node node in path)
            {
                Gizmos.DrawCube(node.worldPosition, Vector3.one * grid.nodeDiameter * 0.9f + Vector3.up * 0.1F);
            }
        }
        else
        {
            //Debug.LogWarning("Path does not exist");
        }
        /*if (grid.grid != null)
        {
            Gizmos.color = Color.yellow;
            Node playerNode = grid.GetNodeByWorldPosition(player.position);
            Gizmos.DrawCube(playerNode.worldPosition, Vector3.one * grid.nodeDiameter * 0.9f + Vector3.up * 0.1F);
            Node goalNode = grid.GetNodeByWorldPosition(goal.position);
            Gizmos.DrawCube(goalNode.worldPosition, Vector3.one * grid.nodeDiameter * 0.9f + Vector3.up * 0.1F);
        }*/
    }
    public List<Node> GetPath(Node startNode, Node finishNode)
    {
        if (!startNode.walkable || !finishNode.walkable)
        {
            return null;
        }
        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();
        open.Add(startNode);

        Node current = startNode;
        while (true)
        {
            open.Remove(current);
            close.Add(current);
            if (current == finishNode)
            {
                return finishNode.GetPath();
            }

            foreach (Node neighbour in current.neighbours)
            {
                if (neighbour == null || !neighbour.walkable || close.Contains(neighbour))
                {
                    continue;
                }
                if (!open.Contains(neighbour) || neighbour.GetNewFCost(current, finishNode) < neighbour.FCost)
                {
                    neighbour.RecalculateFCost(current, finishNode);
                    neighbour.parent = current;
                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }

            }
            Node minFCost = open[0];//Если на этой строке появляется ошибка, то просто нет соседей рядом
            foreach (Node node in open)
            {
                if (node.FCost < minFCost.FCost) minFCost = node;
            }
            current = minFCost;

        }
    }
    public List<Node> GetPath(Vector3 startNode, Vector3 finishNode)
    {
        return GetPath(grid.GetNodeByWorldPosition(startNode), grid.GetNodeByWorldPosition(finishNode));
    }

}
