using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Will load a new scene when reached, requesting a specific Spawn point in the new scene by name.
/// (In this simplified project, the user need to manually type the right spawn point name and there is no check if the
/// spawn point exist. In a real project, editor scripts could be written to validate the entry or over a dropdown instead)
/// </summary>
public class Teleporter : ClickableObject
{
    public int TargetScene;
    public string TargetSpawnPoint;

    public override void Reached(CharacterControl player)
    {
        base.Reached(player);
        LevelSystem.LoadInScene(TargetScene, TargetSpawnPoint);
    }
}
