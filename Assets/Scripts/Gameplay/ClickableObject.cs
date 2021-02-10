using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple script handling object that are clickable by the user.
//Reached is called by the character player when they finally reach the object, so subclass can override the method to
//handle what happen when the player arrive at the object.
public class ClickableObject : MonoBehaviour
{
    private Collider m_Collider;
    private MeshRenderer m_MeshRenderer;

    private int m_IntensityID;
    
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        m_Collider = GetComponentInChildren<Collider>();

        m_IntensityID = Shader.PropertyToID("_RimIntensity");
        
        m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
        m_MeshRenderer.material.SetFloat(m_IntensityID, 0);
    }

    public float GetDistance(Vector3 point)
    {
        var p = m_Collider.ClosestPoint(point);

        return Vector3.Distance(p, point);
    }

    public virtual void Reached(CharacterControl player)
    {
    }

    //will set the intensity of the rim lighting shader that show the object is clickable
    //called by the Character controller script when hovering over the object
    public void Highlight(bool show)
    {
        if(m_MeshRenderer != null)
            m_MeshRenderer.material.SetFloat(m_IntensityID, show ? 5.0f : 0.0f);
    }
}
