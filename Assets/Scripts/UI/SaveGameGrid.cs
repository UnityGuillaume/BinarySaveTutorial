using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Some simple UI that will display handle displaying/updating a grid of save file entry.
/// </summary>
public class SaveGameGrid : MonoBehaviour
{
    public SaveEntry[] Entries;
    public ConfirmationPopup ConfirmationPopup;
    
    private bool m_IsLoading;
    private List<SaveSystem.SaveHeaderData> m_SaveInfo;
    private Action<string> m_OnSelected;
    
    public void Show(bool isLoading, Action<string> onSelect)
    {
        m_OnSelected = onSelect;
        m_IsLoading = isLoading;
        gameObject.SetActive(true);

        //Getting the save info isn't a "free" operation as we need to open and read the save file. We only need the
        //save file header though (see SaveSystem comments) so no matter the save file size this will be a constant operation
        m_SaveInfo = SaveSystem.GetSaveInfo();

        for (int i = 0; i < Entries.Length; ++i)
        {
            var i1 = i;
            Entries[i].EntryButton.onClick.RemoveAllListeners();
            Entries[i].EntryButton.onClick.AddListener(() => ButtonClicked(i1));

            if (m_SaveInfo[i] != null)
            {
                Entries[i].SaveDataText.text = m_SaveInfo[i].Time;
                Entries[i].SaveImage.texture = m_SaveInfo[i].Texture;
            }
            else
            {
                Entries[i].SaveDataText.text = "Empty";
            }
        }
    }

    void ButtonClicked(int id)
    {
        if (!m_IsLoading)
        {
            ConfirmationPopup.Open((valid) => { m_OnSelected.Invoke(valid ? $"SaveGame{id}.usg" : null); gameObject.SetActive(false); });
        }
        else
        {
            //if save info is null, no save in that slot, ignore that click
            m_OnSelected.Invoke(m_SaveInfo[id] != null ? $"SaveGame{id}.usg" : null);
            gameObject.SetActive(false);
        }
    }
}
