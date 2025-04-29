using System.Collections.Generic;
using UnityEngine;

public class HexNode : Node
{
    [SerializeField] Offset offset;
    [SerializeField] Cubic cubic;
    
    public HexNode Set(Offset offset)
    {
        this.offset = offset;
        this.cubic = offset.ToCubic();
        return this;
    }

    public override float HeuristicDist(Node other)
    {
        if (other is HexNode node)
        {
            Cubic a = new Offset() { col = offset.col, row = -offset.row }.ToCubic();
            Cubic b = new Offset() { col = node.offset.col, row = -node.offset.row }.ToCubic();
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s)) / 2;
        }
        return 0.0f;
    }
    /*readonly Cubic[] dir = new Cubic[6]
    {
        new(){q=1, r=0, s=-1}, new(){q=1, r=-1, s=0}, new(){q=0, r=-1, s=1},
        new(){q=-1, r=0, s=1}, new(){q=-1, r=1, s=0}, new(){q=0, r=1, s=-1}
    };
    public override IEnumerable<Node> GetNeighbors()
    {
        for(int i = 0; i < 6; i++)
        {
            Node tmp = HexNodeManager.Instance.GetNodeAt((offset.ToCubic() + dir[i]).ToOffset());
            if (tmp != null) yield return tmp;
        }
    }*/
    readonly Offset[] dir = new Offset[12]
    {
        new(){col = 1, row = -1}, new(){col = 1, row = 0}, new(){col = 0, row = 1},
        new(){col = -1, row = 0}, new(){col = -1, row = -1}, new(){col = 0, row = -1},
        new(){col = 1, row = 0}, new(){col = 1, row = 1}, new(){col = 0, row = 1},
        new(){col = -1, row = 1}, new(){col = -1, row = 0}, new(){col = 0, row = -1}
    };
    public override IEnumerable<Node> GetNeighbors()
    {
        if(offset.col % 2 == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                Node tmp = HexNodeManager.Instance.GetNodeAt(offset + dir[i]);
                if (tmp != null) yield return tmp;
            }
        }
        else
        {
            for (int i = 6; i < 12; i++)
            {
                Node tmp = HexNodeManager.Instance.GetNodeAt(offset + dir[i]);
                if (tmp != null) yield return tmp;
            }
        }
    }
}
[System.Serializable]
public struct Cubic
{
    public int q, r, s;
    public Offset ToOffset()
    {
        int col = q;
        int row = r + (q + (q & 1)) / 2;
        return new() { col = col, row = row };
    }
    public static Cubic operator -(Cubic a, Cubic b)
    {
        return new Cubic() { q = a.q - b.q, r = a.r - b.r, s = a.s - b.s };
    }
    public static Cubic operator +(Cubic a, Cubic b)
    {
        return new Cubic() { q = a.q + b.q, r = a.r + b.r, s = a.s + b.s };
    }
}
[System.Serializable]
public struct Offset
{
    public int col, row;
    public Cubic ToCubic()
    {
        int q = col;
        int r = row - (col + (col & 1)) / 2;
        return new() { q = q, r = r, s = -q - r };
    }
    public static Offset operator -(Offset a, Offset b)
    {
        return new Offset() { col = a.col - b.col, row = a.row - b.row };
    }
    public static Offset operator +(Offset a, Offset b)
    {
        return new Offset() { col = a.col + b.col, row = a.row + b.row };
    }
}