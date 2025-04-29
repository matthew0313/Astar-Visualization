using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node_Display : MonoBehaviour
{
    [SerializeField] Animator anim;

    [Header("Numbers")]
    [SerializeField] TMP_Text gText;
    [SerializeField] TMP_Text hText, fText;

    [Header("Line")]
    [SerializeField] Transform line;

    Node origin;
    public void OnAwake()
    {
        origin = GetComponent<Node>();
        origin.onNumberUpdate += UpdateNumbers;
        origin.onModeUpdate += UpdateModes;
        origin.onPrevUpdate += UpdateLine;
    }

    StringBuilder sb = new();
    void UpdateNumbers()
    {
        sb.Clear();
        sb.Append("G: ");
        sb.Append(Mathf.Round(origin.g * 10.0f) / 10.0f);
        gText.text = sb.ToString();

        sb.Clear();
        sb.Append("H: ");
        sb.Append(Mathf.Round(origin.h * 10.0f) / 10.0f);
        hText.text = sb.ToString();

        sb.Clear();
        sb.Append("F: ");
        sb.Append(Mathf.Round(origin.f * 10.0f) / 10.0f);
        fText.text = sb.ToString();
    }
    readonly int modeID = Animator.StringToHash("Mode");
    void UpdateModes()
    {
        anim.SetInteger(modeID, (int)origin.mode);
    }
    void UpdateLine()
    {
        line.gameObject.SetActive(origin.prev != null);

        Vector2 dir = (origin.prev.transform.position - origin.transform.position).normalized;
        line.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        line.localScale = new Vector2(Vector2.Distance(line.position, origin.prev.transform.position), 1.0f);
    }
}