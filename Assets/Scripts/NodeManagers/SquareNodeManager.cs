using UnityEngine;
using System.Collections.Generic;

public class SquareNodeManager : NodeManager
{
    public static SquareNodeManager Instance { get; private set; }
    public SquareNodeManager() => Instance = this;

    [Header("Square Grid")]
    [SerializeField] IntegerHook xGridSizeHook;
    [SerializeField] IntegerHook yGridSizeHook;
    int xGridSize => xGridSizeHook.value;
    int yGridSize => yGridSizeHook.value;

    [Header("Node Spawning")]
    [SerializeField] SquareNode nodePrefab;
    [SerializeField] Transform nodeAnchor;
    [SerializeField] Vector2 nodeSpacing;

    readonly List<List<SquareNode>> nodes = new();

    new protected void Awake()
    {
        base.Awake();
        xGridSizeHook.AssignCallback(UpdateGrid);
        yGridSizeHook.AssignCallback(UpdateGrid);
        UpdateGrid();
    }
    private void OnDestroy()
    {
        xGridSizeHook.RemoveCallback(UpdateGrid);
        yGridSizeHook.RemoveCallback(UpdateGrid);
    }
    void UpdateGrid()
    {
        int i;
        for (i = 0; i < yGridSize; i++)
        {
            if (nodes.Count <= i) nodes.Add(new());
            int k;
            for (k = 0; k < xGridSize; k++)
            {
                if (nodes[i].Count <= k) nodes[i].Add(CreateNode(k, i).Set(k, i));
                nodes[i][k].gameObject.SetActive(true);
            }
            for(; k < nodes[i].Count; k++) nodes[i][k].gameObject.SetActive(false);
        }
        for(; i < nodes.Count; i++)
        {
            for (int k = 0; k < nodes[i].Count; i++) nodes[i][k].gameObject.SetActive(false);
        }
        PlayerCamera.Instance.moveEdgeMin = nodes[0][0].transform.position;
        PlayerCamera.Instance.moveEdgeMax = nodes[yGridSize-1][xGridSize-1].transform.position;
    }
    SquareNode CreateNode(int x, int y)
    {
        SquareNode tmp = Instantiate(nodePrefab, nodeAnchor);
        tmp.transform.localPosition = new Vector2(nodeSpacing.x * x, nodeSpacing.y * y);
        return tmp;
    }
    public SquareNode GetNodeAt(int x, int y)
    {
        if (x < 0 || x >= xGridSize || y < 0 || y >= yGridSize) return null;
        return nodes[y][x];
    }

    protected override IEnumerable<Node> GetNodes()
    {
        for(int i = 0; i < yGridSize; i++)
        {
            for (int k = 0; k < xGridSize; k++) yield return nodes[i][k];
        }
    }
}