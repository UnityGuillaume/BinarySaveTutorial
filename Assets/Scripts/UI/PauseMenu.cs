using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu s_Instance;
    public SaveGameGrid Grid;

    private void Awake()
    {
        s_Instance = this;
        s_Instance.gameObject.SetActive(false);
    }

    public static void Toggle()
    {
        s_Instance.gameObject.SetActive(!s_Instance.gameObject.activeSelf);

        if (!s_Instance.gameObject.activeSelf)
            s_Instance.Grid.gameObject.SetActive(false);
    }

    public void Save()
    {
        Grid.Show(false, (saveFile) =>
        {
            s_Instance.gameObject.SetActive(false);
            
            if (saveFile != null)
            {
                
                SaveSystem.Save(saveFile, null);
            }
        });
    }

    public void Load()
    {
        Grid.Show(true, (saveFile) =>
        {
            if (saveFile != null)
            {
                EntryPoint.LoadingPanel.Fade(1.0f, () => { SaveSystem.Load(saveFile); });
            }

            s_Instance.gameObject.SetActive(false);
        });
    }

    public void Quit()
    {
        s_Instance.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
