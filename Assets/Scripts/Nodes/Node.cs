using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Node_Display))]
public abstract class Node : MonoBehaviour
{
    Node_Display display;

    [SerializeField] float m_g = 0, m_h = 0;
    public float g
    {
        get => m_g;
        set
        {
            m_g = value;
            onNumberUpdate?.Invoke();
        }
    }
    public float h
    {
        get => m_h;
        set
        {
            m_h = value;
            onNumberUpdate?.Invoke();
        }
    }
    public float f => g + h;
    public Action onNumberUpdate;

    Node m_prev;
    public Node prev
    {
        get => m_prev;
        set
        {
            if (m_prev != value)
            {
                m_prev = value;
                onPrevUpdate?.Invoke();
            }
        }
    }
    public Action onPrevUpdate;
    public bool canPass => mode != NodeMode.Blocked;
    public abstract float HeuristicDist(Node other);
    public abstract IEnumerable<Node> GetNeighbors();

    [SerializeField] NodeMode m_mode;
    public NodeMode mode
    {
        get => m_mode;
        set
        {
            if (m_mode != value)
            {
                m_mode = value;
                onModeUpdate?.Invoke();
            }
        }
    }
    public Action onModeUpdate;
    private void OnValidate()
    {
        onNumberUpdate?.Invoke();
        onModeUpdate?.Invoke();
    }
    private void Awake()
    {
        display = GetComponent<Node_Display>();
        display.OnAwake();
    }
    private void OnEnable()
    {
        mode = NodeMode.None;
        g = 0; h = 0;
    }
}
[System.Serializable]
public enum NodeMode
{
    None = 0,
    Listed = 1,
    Checked = 2,
    Path = 3,
    Blocked = 4
}