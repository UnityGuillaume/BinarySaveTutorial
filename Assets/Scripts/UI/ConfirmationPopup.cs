using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour
{
    private System.Action<bool> m_OnClose;

    public void Open(System.Action<bool> onClose)
    {
        m_OnClose = onClose;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        m_OnClose.Invoke(true);
        Close();
    }

    public void Cancel()
    {
        m_OnClose.Invoke(false);
        Close();
    }
}
