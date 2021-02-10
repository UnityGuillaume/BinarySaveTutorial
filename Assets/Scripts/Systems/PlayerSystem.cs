using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This is the entry point to everything player character related : it will instatiate the player prefab when needed
/// And have static access to save and load player related data.
/// </summary>
public class PlayerSystem
{
    private static CharacterControl s_PlayerInstance;
    private static GameObject s_Root;

    static void InstantiatePlayer()
    {
        s_Root = Object.Instantiate(EntryPoint.Instance.PlayerPrefab);
        s_PlayerInstance = s_Root.GetComponentInChildren<CharacterControl>();
        Object.DontDestroyOnLoad(s_Root); 
    }

    public static void SpawnAt(Transform pos)
    {
        if(s_PlayerInstance == null)
            InstantiatePlayer();
        
        s_PlayerInstance.MoveTo(pos.position, pos.rotation);
    }
    
    //In our simple example we only load the player data, but this could be were we load every data related to the player
    //gameobject like the content of a player stash, or the unlocked skills etc.
    public static void LoadData(BinaryReader reader)
    {
        if(s_PlayerInstance == null)
            InstantiatePlayer();
        
        s_PlayerInstance.LoadPlayerData(reader);
    }

    public static void SaveData(BinaryWriter writer)
    {
        s_PlayerInstance.SavePlayerData(writer);
    }

    public static void Cleanup()
    {
        Object.Destroy(s_Root);
        s_PlayerInstance = null;
    }
}
