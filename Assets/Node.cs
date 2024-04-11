using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public Node parent { get; set; }
    public Node[] neighbours { get; set; }
    //Distance from the starting node
    private float GCost {  get; set; }
    //Distance from the ending node
    private float HCost { get; set; }
    //The sum of GCost and HCost
    public float FCost
    {
        get
        {
            return GCost + HCost;
        }
    }
    /// <summary>
    /// Return FCost for new parameters
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parent"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public float GetNewFCost(Node parent, Node goal)
    {
        float newGCost = parent.GCost + GetDistanceBetweenNodes(this, parent);
        float newHCost = GetDistanceBetweenNodes(goal, this);
        return newGCost + newHCost;
    }
    public void RecalculateFCost(Node parent, Node goal)
    {
        GCost = parent.GCost + GetDistanceBetweenNodes(this, parent);
        HCost = GetDistanceBetweenNodes(goal, this);
    }

    static public float GetDistanceBetweenNodes(Node node1, Node node2)
    {
        return Vector3.Distance(node1.worldPosition, node2.worldPosition);
    }

    public Node(bool _walkable, Vector3 _worldPosition)
    {
        neighbours = new Node[4];
        walkable = _walkable;
        worldPosition = _worldPosition;
    }

    public List<Node> GetPath()
    {
        List<Node> path = new List<Node>();
        Node current = this;
        path.Add(this);
        while (current.parent != null)
        {
            Debug.Log(1);
            path.Add(current);
            current = current.parent;
        }
        return path;
    }
}
