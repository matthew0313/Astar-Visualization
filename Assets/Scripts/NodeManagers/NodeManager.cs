using UnityEngine;
using System.Collections.Generic;
using MEC;

public abstract class NodeManager : MonoBehaviour
{
    [SerializeField] IntegerHook nodePickModeHook;
    [SerializeField] FloatHook tickRateHook;
    NodePickMode nodePickMode => (NodePickMode)nodePickModeHook.value;
    float tickRate => tickRateHook.value;
    public bool isSearching { get; private set; } = false;

    [Header("Indicators")]
    [SerializeField] GameObject startNodeIndicator;
    [SerializeField] GameObject endNodeIndicator;

    CoroutineHandle searching;
    Node m_startNode, m_endNode;
    protected Node startNode
    {
        get => m_startNode;
        set
        {
            if (m_startNode != value)
            {
                m_startNode = value;
                if (m_startNode != null)
                {
                    startNodeIndicator.transform.position = m_startNode.transform.position;
                    startNodeIndicator.SetActive(true);
                }
                else startNodeIndicator.SetActive(false);
            }
        }
    }
    protected Node endNode
    {
        get => m_endNode;
        set
        {
            if (m_endNode != value)
            {
                m_endNode = value;
                if (m_endNode != null)
                {
                    endNodeIndicator.transform.position = m_endNode.transform.position;
                    endNodeIndicator.SetActive(true);
                }
                else endNodeIndicator.SetActive(false);
            }
        }
    }
    protected abstract IEnumerable<Node> GetNodes();
    public void Search()
    {
        if (startNode == null || endNode == null || isSearching) return;
        isSearching = true;

        Clear();
        searching = Timing.RunCoroutine(Searching());
    }
    public void PauseSearch()
    {
        if (!isSearching) return;
        Timing.PauseCoroutines(searching);
    }
    public void EndSearch()
    {
        if (isSearching) return;
        Timing.KillCoroutines(searching);
        isSearching = false;
    }
    public void Clear()
    {
        foreach (var i in GetNodes())
        {
            if (i.mode != NodeMode.Blocked) i.mode = NodeMode.None;
        }
    }
    public void ClearWalls()
    {
        foreach(var i in GetNodes())
        {
            i.mode = NodeMode.None;
        }
    }
    protected void Awake()
    {
        startNodeIndicator.SetActive(false);
        endNodeIndicator.SetActive(false);
    }
    protected void Update()
    {
        if (isSearching) return;
        switch (nodePickMode)
        {
            case NodePickMode.StartNode:
                if (Input.GetMouseButtonDown(0) && UIScanner.ScanUI(Input.mousePosition).Count <= 0)
                {
                    Collider2D tmp = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (tmp != null && tmp.TryGetComponent(out Node node))
                    {
                        if (node == endNode) return;
                        if(node.mode == NodeMode.Blocked) node.mode = NodeMode.None;
                        startNode = node;
                    }
                }
                break;
            case NodePickMode.EndNode:
                if (Input.GetMouseButtonDown(0) && UIScanner.ScanUI(Input.mousePosition).Count <= 0)
                {
                    Collider2D tmp = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (tmp != null && tmp.TryGetComponent(out Node node))
                    {
                        if (node == startNode) return;
                        if(node.mode == NodeMode.Blocked) node.mode = NodeMode.None;
                        endNode = node;
                    }
                }
                break;
            case NodePickMode.Wall:
                if (Input.GetMouseButton(0) && UIScanner.ScanUI(Input.mousePosition).Count <= 0)
                {
                    Collider2D tmp = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (tmp != null && tmp.TryGetComponent(out Node node))
                    {
                        node.mode = NodeMode.Blocked;
                    }
                }
                else if (Input.GetMouseButton(1) && UIScanner.ScanUI(Input.mousePosition).Count <= 0)
                {
                    Collider2D tmp = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (tmp != null && tmp.TryGetComponent(out Node node))
                    {
                        if(node.mode == NodeMode.Blocked) node.mode = NodeMode.None;
                    }
                }
                break;
        }
    }
    readonly List<Node> toSearch = new(), searched = new();
    IEnumerator<float> Searching()
    {
        toSearch.Clear(); searched.Clear();

        startNode.mode = NodeMode.Listed;
        startNode.g = 0;
        startNode.h = startNode.HeuristicDist(endNode);
        startNode.prev = null;
        toSearch.Add(startNode);

        while(toSearch.Count > 0)
        {
            yield return Timing.WaitForSeconds(tickRate);
            toSearch.Sort((a, b) =>
            {
                if (a.f < b.f) return -1;
                else if (a.f > b.f) return 1;
                else
                {
                    if (a.h < b.h) return -1;
                    else if (a.h > b.h) return 1;
                    else return 0;
                }
            });
            Node searching = toSearch[0];
            if (searching == endNode)
            {
                Node tmp = searching;
                while(tmp != null)
                {
                    tmp.mode = NodeMode.Path;
                    tmp = tmp.prev;
                }
                break;
            }
            else
            {
                foreach(var i in searching.GetNeighbors())
                {
                    if (i.mode == NodeMode.Blocked || searched.Contains(i)) continue;
                    if (!toSearch.Contains(i))
                    {
                        i.prev = searching;
                        i.g = searching.g + i.HeuristicDist(searching);
                        i.h = i.HeuristicDist(endNode);
                        i.mode = NodeMode.Listed;
                        toSearch.Add(i);
                    }
                    else if(i.g > searching.g + i.HeuristicDist(searching))
                    {
                        i.prev = searching;
                        i.g = searching.g + i.HeuristicDist(searching);
                    }
                }
            }
            searching.mode = NodeMode.Checked;
            searched.Add(searching);
            toSearch.Remove(searching);
        }
        isSearching = false;
    }
}
public enum NodePickMode
{
    StartNode = 0,
    EndNode = 1,
    Wall = 2
}