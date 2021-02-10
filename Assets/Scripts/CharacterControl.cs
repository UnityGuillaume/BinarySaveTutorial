using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Handle everything related to the player character : input handling, movement, inventory and saving/loading its data
/// </summary>
public class CharacterControl : MonoBehaviour
{
    public CinemachineVirtualCamera VCam;
    public Camera GameCamera;
    
    public List<Item> Inventory => m_Inventory;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private int m_TerrainLayer;
    private int m_ItemLayer;

    private int m_SpeedParamID;

    private List<Item> m_Inventory;

    private ClickableObject m_PreviousHighlighted;
    private ClickableObject m_TargetObject;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponentInChildren<Animator>();

        m_SpeedParamID = Animator.StringToHash("Speed");

        m_ItemLayer = LayerMask.NameToLayer("Item"); 
        m_TerrainLayer = LayerMask.NameToLayer("Terrain");

        Init();
    }

    void Start()
    {
        
    }

    void Init()
    {
        m_Inventory = new List<Item>();
        UIHandler.Instance.UpdateInventory(this);
    }
    
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            var ray = GameCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            
            if (Physics.Raycast(ray, out hit, 1000.0f, 1 << m_TerrainLayer | 1 << m_ItemLayer))
            {
                ClickableObject itm = hit.collider.GetComponentInChildren<ClickableObject>();

                if(m_PreviousHighlighted != null)
                    m_PreviousHighlighted.Highlight(false);
                
                if (itm != null)
                {
                    itm.Highlight(true);
                    m_PreviousHighlighted = itm;
                    
                    if(Input.GetMouseButtonDown(0))
                        m_TargetObject = itm;
                }
                else
                {
                    if(Input.GetMouseButtonDown(0))
                        m_TargetObject = null;
                }

                if(Input.GetMouseButtonDown(0))
                    m_Agent.SetDestination(hit.point);
            }
        }

        if (m_TargetObject != null)
        {
            float distance = m_TargetObject.GetDistance(transform.position);
            if (distance <= 1.0f)
            {
                m_TargetObject.Reached(this);
                m_Agent.ResetPath();
                m_TargetObject = null;
            }
        }
        
        m_Animator.SetFloat(m_SpeedParamID, m_Agent.velocity.magnitude / m_Agent.speed);
        
        
        if(Input.GetKeyDown(KeyCode.Escape))
            PauseMenu.Toggle();
    }

    public void AddItemToInventory(Item item)
    {
        m_Inventory.Add(item);
        UIHandler.Instance.UpdateInventory(this);
    }
    
    public void MoveTo(Vector3 pos, Quaternion rot)
    {
        Vector3 offset = pos - transform.position;
        
        //we move the agent too, otherwise its old position could override our new position when loading into the same scene
        m_Agent.Warp(pos);
        
        transform.SetPositionAndRotation(pos, rot);
        VCam.OnTargetObjectWarped(VCam.Follow, offset);
    }

    //Called by the PlayerSystem when a save is triggered.
    public void SavePlayerData(BinaryWriter writer)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        
        //Binary writer don't have any overload for Unity type like Vector3 or Quaternion. If you save a lot of those in
        //your project, it can be beneficial to write an extension to BinaryWriter to handle those couple of lines automatically
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);
        
        writer.Write(rot.x);
        writer.Write(rot.y);
        writer.Write(rot.z);
        writer.Write(rot.w);

        //saving inventory, by saving first how many item there is in the list, then each item unique ID back to back.
        writer.Write(m_Inventory.Count);
        for(int i = 0; i < m_Inventory.Count; ++i)
            writer.Write(m_Inventory[i].UniqueID);
    }

    //called by the PlayerSystem when a save is loaded 
    public void LoadPlayerData(BinaryReader reader)
    {
        //We reinit the player, because if the load the game in the middle of an already loading game, we need a "clean"
        //version of the player so we don't have left over object or settings from that previous game.
        Init();
        
        Vector3 pos = new Vector3(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle());
        
        Quaternion rot = new Quaternion(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle());
        
        //reading inventory
        //we added inventory mid development, so we check the version of the save we read. If the save being read is
        //below version 2 (when the inventory was added) we instead leave the empty list created in Init, as the data
        //is not in the save file.
        if (SaveSystem.SaveData.Version >= 2)
        {
            int size = reader.ReadInt32();
            for (int i = 0; i < size; ++i)
            {
                string id = reader.ReadString();
                var itm = EntryPoint.Instance.ItemDB.GetItem(id);
                if (itm != null)
                {
                    m_Inventory.Add(itm);
                }
                else
                {
                    Debug.LogError("Unknown Item in save file");
                }
            }
        }

        MoveTo(pos, rot);

       UIHandler.Instance.UpdateInventory(this);
    }
}
