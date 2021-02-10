using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : ClickableObject
{
    public string UniqueName;
    public Item Key; 
    
    private void OnEnable()
    {
        //When we load into the scene in which that door is, we need to remove it if it was previously open.
        //If the flag composed of levelname_uniquename is true (if it don't exist that function return false) that mean
        //it was indeed open, so we can destroy the door (depending on the game, this could be instead setting the open
        // animation state, or changing the gameobject used etc...)
        if (LevelSystem.Instance.GetFlag(UniqueName))
        {
            Destroy(gameObject);
        }
    }

    public override void Reached(CharacterControl player)
    {
        base.Reached(player);

        if (player.Inventory.Find(Item => Item.UniqueID == Key.UniqueID) != null)
        {
            Destroy(gameObject);
            
            //we opened that door, so we set the flag with the door UniqueName to true. That way if we load back into that
            //scene (either from loading a save file or exiting and coming back to that scene) we can check the flag value
            //and know that the door had been open (as loading a scene will set every object in the state they were
            //when the scene file was saved in the editor)
            LevelSystem.Instance.SaveFlag(UniqueName, true);
        }
    }
}
