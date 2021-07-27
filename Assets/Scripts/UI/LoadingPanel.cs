using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick helper that will fade a black screen in and out, with the possibility to give a function to call when the fade
/// in finish (so the screen is full dark)
/// </summary>
public class LoadingPanel : MonoBehaviour
{
    public Image TargetImage;
    
    private System.Action m_Callback;
    private int m_Direction = 1;
    private float m_FadeTime;
    private float m_CurrentTime;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Color c = TargetImage.color;

        if (m_Direction > 0)
        {
            m_CurrentTime += Time.unscaledDeltaTime;

            if (m_CurrentTime >= m_FadeTime)
            {
                m_Direction = -1;

                m_Callback?.Invoke();
                m_Callback = null;
            }
        }
        else
        {
            m_CurrentTime -= Time.unscaledDeltaTime;
 
            if (m_CurrentTime <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        c.a = Mathf.Lerp(0.0f, 1.0f, Mathf.Clamp01(m_CurrentTime/m_FadeTime));
        
        TargetImage.color = c;
    }

    public void Fade(float fadeTime, System.Action callbackOnEnd)
    {
        //we are already fading if active, keep on the fade does not reset it
        if (!gameObject.activeInHierarchy)
        {
            m_FadeTime = fadeTime;
            m_CurrentTime = 0;
            m_Direction = 1;
            
            Color c = TargetImage.color;
            c.a = 0.0f;
            TargetImage.color = c;
            
            gameObject.SetActive(true);
        }

        if (m_Direction < 0) //we are already fading out, so we call the callback right away as the fadeout already happened
            callbackOnEnd();
        else
            m_Callback += callbackOnEnd;
    }
}
