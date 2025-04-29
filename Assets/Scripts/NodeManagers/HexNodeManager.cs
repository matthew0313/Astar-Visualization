using UnityEngine;
using System.Collections.Generic;

public class HexNodeManager : NodeManager
{
    public static HexNodeManager Instance { get; private set; }
    public HexNodeManager() => Instance = this;

    [Header("Hex Grid")]
    [SerializeField] IntegerHook columnsHook;
    [SerializeField] IntegerHook rowsHook;
    int columns => columnsHook.value;
    int rows => rowsHook.value;

    [Header("Node Spawning")]
    [SerializeField] HexNode nodePrefab;
    [SerializeField] Transform nodeAnchor;
    [SerializeField] float nodeSize;

    readonly List<List<HexNode>> nodes = new();
    new protected void Awake()
    {
        base.Awake();
        columnsHook.AssignCallback(UpdateGrid);
        rowsHook.AssignCallback(UpdateGrid);
        UpdateGrid();
    }
    private void OnDestroy()
    {
        columnsHook.RemoveCallback(UpdateGrid);
        rowsHook.RemoveCallback(UpdateGrid);
    }
    void UpdateGrid()
    {
        int i;
        for (i = 0; i < rows; i++)
        {
            if (nodes.Count <= i) nodes.Add(new());
            int k;
            for (k = 0; k < columns; k++)
            {
                if (nodes[i].Count <= k) nodes[i].Add(CreateNode(k, i).Set(new Offset() { col = k, row = i }));
                nodes[i][k].gameObject.SetActive(true);
            }
            for (; k < nodes[i].Count; k++) nodes[i][k].gameObject.SetActive(false);
        }
        for (; i < nodes.Count; i++)
        {
            for (int k = 0; k < nodes[i].Count; i++) nodes[i][k].gameObject.SetActive(false);
        }
        PlayerCamera.Instance.moveEdgeMin = nodes[0][0].transform.position;
        PlayerCamera.Instance.moveEdgeMax = nodes[rows - 1][columns - 1].transform.position;
    }
    HexNode CreateNode(int x, int y)
    {
        HexNode tmp = Instantiate(nodePrefab, nodeAnchor);
        tmp.transform.localPosition = OffsetToPos(new Offset() { col = x, row = y });
        return tmp;
    }
    Vector2 OffsetToPos(Offset offset)
    {
        return new Vector2(nodeSize * 3 / 2 * offset.col, nodeSize * Mathf.Sqrt(3) * offset.row + offset.col % 2 * nodeSize * Mathf.Sqrt(3) / 2);
    }
    public HexNode GetNodeAt(Offset offset)
    {
        if (offset.col < 0 || offset.col >= columns || offset.row < 0 || offset.row >= rows) return null;
        return nodes[offset.row][offset.col];
    }

    protected override IEnumerable<Node> GetNodes()
    {
        for(int i = 0; i < rows; i++)
        {
            for (int k = 0; k < columns; k++) yield return nodes[i][k];
        }
    }
}