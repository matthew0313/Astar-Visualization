using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    List<T> body = new();
    readonly Func<T, T, bool> comparator;
    public PriorityQueue(Func<T, T, bool> comparator)
    {
        this.comparator = comparator;
    }
    public void Insert(T item)
    {
        body.Add(item);
        CheckUp(body.Count - 1 + 1);
    }
    public void Pop()
    {
        if (body.Count == 0)
        {
            Debug.LogWarning("Tried to pop an empty priority queue");
            return;
        }
        body[0] = body[body.Count - 1];
        body.RemoveAt(body.Count - 1);
        CheckDown(0 + 1);
    }
    public T Top()
    {
        return body[0];
    }
    void CheckUp(int index)
    {
        if (index == 1) return;
        if (comparator.Invoke(body[index - 1], body[index / 2 - 1]))
        {
            T tmp = body[index / 2 - 1];
            body[index / 2 - 1] = body[index - 1];
            body[index - 1] = tmp;

            CheckUp(index / 2);
        }
    }
    void CheckDown(int index)
    {
        if(index * 2 < body.Count + 1)
        {
            if (comparator.Invoke(body[index * 2 - 1], body[index - 1]))
            {
                T tmp = body[index * 2 - 1];
                body[index * 2 - 1] = body[index - 1];
                body[index - 1] = tmp;

                CheckDown(index * 2); return;
            }
        }
        
        if(index * 2 + 1 < body.Count + 1)
        {
            if (comparator.Invoke(body[index * 2 + 1 - 1], body[index - 1]))
            {
                T tmp = body[index * 2 + 1- 1];
                body[index * 2 + 1 - 1] = body[index - 1];
                body[index - 1] = tmp;

                CheckDown(index * 2 + 1); return;
            }
        }
    }
}