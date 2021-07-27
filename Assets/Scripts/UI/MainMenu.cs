using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SaveGameGrid Grid;

    private void Awake()
    {
        EntryPoint.Create();
    }

    public void New()
    {
        LevelSystem.LoadInScene(1, "level1_start");
    }

    public void Load()
    {
        Grid.Show(true, (saveFile) =>
        {
            SaveSystem.Load(saveFile);
        });
    }

    public void Exit()
    {
        Application.Quit();
    }
}
