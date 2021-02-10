using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SaveLoad/Item", fileName = "Item.asset")]
public class Item : ScriptableObject
{
    public string UniqueID;
    public string DisplayName;
    public Sprite Icone;
    public GameObject Prefab;
}
