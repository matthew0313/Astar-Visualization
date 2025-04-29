using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class SquareNode : Node
{
    public int x { get; private set; }
    public int y { get; private set; }
    public SquareNode Set(int x, int y)
    {
        this.x = x;
        this.y = y;
        return this;
    }

    public override float HeuristicDist(Node other)
    {
        if (other is SquareNode node)
        {
            float tot = 0;
            float x = Mathf.Abs(node.x - this.x), y = Mathf.Abs(node.y - this.y);
            tot += Mathf.Max(x, y) - Mathf.Min(x, y);
            tot += Mathf.Min(x, y) * Mathf.Sqrt(2);
            return tot;
        }
        return 0.0f;
    }
    readonly int[] dirX = new int[8] { 0, 1, 0, -1, 1, 1, -1, -1 };
    readonly int[] dirY = new int[8] { 1, 0, -1, 0, 1, -1, 1, -1 };
    public override IEnumerable<Node> GetNeighbors()
    {
        for(int i = 0; i < 8; i++)
        {
            Node tmp = SquareNodeManager.Instance.GetNodeAt(x + dirX[i], y + dirY[i]);
            if (tmp != null) yield return tmp;
        }
    }
}