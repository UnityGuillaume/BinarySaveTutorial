using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define a point where the player will be placed when a the scene load.
/// Scene can have multiple SpawnPoint, and the LevelSystem will set the right one as active depending on what the game
/// asked
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    private static string s_TargetSpawn;
    private static SpawnPoint s_ActiveSpawn;
    private static SpawnPoint s_DefaultSpawn;

    private static List<SpawnPoint> s_Spawns = new List<SpawnPoint>();
    
    public string UniqueName;
    public bool DefaultSpawn;
    
    private void OnEnable()
    {
        s_Spawns.Add(this);
        
        if (UniqueName == s_TargetSpawn)
            s_ActiveSpawn = this;

        if (DefaultSpawn)
            s_DefaultSpawn = this;
    }

    private void OnDisable()
    {
        if (s_ActiveSpawn == this)
            s_ActiveSpawn = null;

        s_Spawns.Remove(this);
    }

    public static SpawnPoint GetActiveSpawnPoint(bool returnDefault)
    {
        if (s_ActiveSpawn == null && returnDefault)
            return s_DefaultSpawn;
        
        return s_ActiveSpawn;
    }

    public static void SetTargetSpawn(string name)
    {
        s_TargetSpawn = name;
    }
}
