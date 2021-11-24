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
            //our main menu will be destroyed when we change scene, so if we were to use StartCoroutine the coroutine
            //would stop as soon as we transition. Instead we use the helper function on the entry point to create a 
            //"permanent" coroutine that survive scene change.
            EntryPoint.StartPermanentCoroutine(SaveSystem.Load(saveFile));
        });
    }

    public void Exit()
    {
        Application.Quit();
    }
}
