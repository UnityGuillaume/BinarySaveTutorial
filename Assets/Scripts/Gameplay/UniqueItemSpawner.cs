using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UniqueItemSpawner : MonoBehaviour
{
    public string UniqueName;
    public LootableObject ObjectToSpawn;

    private bool m_WasLooted = false;

    protected void Start()
    {
        
        //When we init the object on scene load, we check if a flag with the object unique name exist.
        //If it does and is true, that mean the item was already looted
        m_WasLooted = LevelSystem.Instance.GetFlag(UniqueName);
        
        if (m_WasLooted)
        {
            //we destroy the spawner, its object was already looter
            Destroy(gameObject);
            return;
        }
        
        //if it wasn't looted already, then we instantiate the prefab that spawner spawns
        var newObj = Instantiate(ObjectToSpawn);
        newObj.transform.position = transform.position;
        newObj.OnLooted += () =>
        {
            //then we register to the OnLooted event of the object. This will be called when the object is picked up by
            //the player. When that happen, we save a flag with the object unique name, so next time that scene is loaded
            //we won't spawn a new object.
            m_WasLooted = true;
            LevelSystem.Instance.SaveFlag(UniqueName, true);
        };
    }
}
