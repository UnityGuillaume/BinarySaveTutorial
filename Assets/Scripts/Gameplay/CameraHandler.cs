using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private const float CameraSpeed = 1.5f;
    private const float SafezoneMargin = 0.3f;
    
    public static CameraHandler Instance { private set; get; }

    private Camera m_Camera;
    private Vector3 m_TargetPosition;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        m_Camera = GetComponent<Camera>();

        if (GetComponent<ScreenshotTaker>() == null)
        {
            gameObject.AddComponent<ScreenshotTaker>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        m_TargetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, Time.deltaTime * CameraSpeed);
    }

    public void TeleportToPlayer(Vector3 playerPosition)
    {
        var pos = transform.position;
        pos.z = playerPosition.z;
        transform.position = pos;
        m_TargetPosition = pos;
    }

    public void UpdateCameraPosition(Vector3 playerPosition)
    {
        var viewportPoint = m_Camera.WorldToViewportPoint(playerPosition);

        if (viewportPoint.x < SafezoneMargin || viewportPoint.x > 1.0f - SafezoneMargin)
        {
            m_TargetPosition.z = playerPosition.z;
        }
    }
}
