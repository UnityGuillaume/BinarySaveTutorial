using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This object will rarely be placed directly in the scene, but instead is created by a spawner that will handle what
/// happen when the object is picked up through the OnLooted event. See UniqueItemSpawner class for an example of saving
/// a state to never spawn that object again, or another example would be to start a timer before spawning a new object etc.
/// </summary>
public class LootableObject : ClickableObject
{
    public Item LootableItem;

    public System.Action OnLooted;
    
    public override void Reached(CharacterControl player)
    {
        player.Take(this);
    }

    public void Looted()
    {
        OnLooted?.Invoke();
        Destroy(gameObject);
    }
}
