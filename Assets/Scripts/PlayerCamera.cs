using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    public PlayerCamera() => Instance = this;

    [SerializeField] Camera cam;
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] float zoomMin = 3.0f, zoomMax = 20.0f;
    [SerializeField] float zoomSensitivity = 1.0f;

    [HideInInspector] public Vector2 moveEdgeMin = Vector2.zero, moveEdgeMax = Vector2.zero;
    private void Update()
    {
        transform.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime);
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, moveEdgeMin.x, moveEdgeMax.x),
            Mathf.Clamp(transform.position.y, moveEdgeMin.y, moveEdgeMax.y));
        transform.position += new Vector3(0, 0, -10.0f);

        cam.orthographicSize += Input.mouseScrollDelta.y * -1.0f * zoomSensitivity;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, zoomMin, zoomMax);
    }
}