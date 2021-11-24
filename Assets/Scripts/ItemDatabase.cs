using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This keep a list of all items in the game, and build a dictionary matching their UniqueId to their instance when the Init
/// function is called (called by the EntryPoint initialization). This allow to have a single entry point to looking up
/// all object in the game (e.g. save file only contain the uniqueId of the item, that is looked up on load in this).
/// The Items list is built in the Editor on the ScriptableObject instance.
/// </summary>
[CreateAssetMenu(fileName = "Itemdb.asset", menuName = "SaveLoad/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public Item[] Items;

    protected Dictionary<string, Item> m_Lookup;
    
    public void Init()
    {
        m_Lookup = new Dictionary<string, Item>();
        foreach (var item in Items)
        {
            m_Lookup[item.UniqueID] = item;
        }
    }

    public Item GetItem(string uniqueID)
    {
        Item itm;
        m_Lookup.TryGetValue(uniqueID, out itm);
        return itm;
    }
}