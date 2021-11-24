using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allow for an access from anywhere to the UI that display the Inventory to easily update it.
/// </summary>
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance => s_Instance;
    private static UIHandler s_Instance;
    
    public Image[] InventoryIcones;

    private void Awake()
    {
        s_Instance = this;
        gameObject.SetActive(false);
    }

    public void UpdateInventory(CharacterControl player)
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        
        for (int i = 0; i < InventoryIcones.Length; ++i)
        {
            if (i < player.Inventory.Count)
            {
                InventoryIcones[i].gameObject.SetActive(true);
                InventoryIcones[i].sprite = player.Inventory[i].Icone;
            }
            else
            {
                InventoryIcones[i].gameObject.SetActive(false);
            }
        }
    }
}
