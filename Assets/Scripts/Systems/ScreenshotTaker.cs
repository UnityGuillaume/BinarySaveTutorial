using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// Helper class that will take a small render of the main camera and encore it to PNG so we can save in the save
/// file the current game view, helping user to remember where that save was made. 
/// </summary>
public class ScreenshotTaker : MonoBehaviour
{
    public static int WIDTH = 320;
    public static int HEIGHT = 180;
    
    private static ScreenshotTaker s_Instance;

    private RenderTexture m_RenderTexture;
    private Texture2D m_Texture;
    private Camera m_Camera;
    
    private void Awake()
    {
        s_Instance = this;
        
        m_RenderTexture = new RenderTexture(WIDTH, HEIGHT, 16, DefaultFormat.LDR);
        m_Texture = new Texture2D(WIDTH, HEIGHT, DefaultFormat.LDR, TextureCreationFlags.None);
        
        m_Camera = GetComponent<Camera>();
    }

    public static byte[] SaveScreen()
    {
        var t = s_Instance.m_Camera.targetTexture;

        s_Instance.m_Camera.targetTexture = s_Instance.m_RenderTexture;
        
        var currentRT = RenderTexture.active;
        RenderTexture.active = s_Instance.m_Camera.targetTexture;

        // Render the camera's view.
        s_Instance.m_Camera.Render();
        
        s_Instance.m_Texture.ReadPixels(new Rect(0, 0, s_Instance.m_Texture.width, s_Instance.m_Texture.height), 0, 0);
        s_Instance.m_Texture.Apply();
        
        RenderTexture.active = currentRT;
        s_Instance.m_Camera.targetTexture = t;
        
        var s = s_Instance.m_Texture.EncodeToPNG();

        return s;
    }
} 
