using System.Collections;
using UnityEngine;

//This class handle referencing all data we may need to access during gameplay. As the name implies, it is the entry point
//to that data. It is used by making a prefab called Main with that script on it placed in the Resources Folder, that way we
//can load it from anywhere at any point.
// In editor we check every access if it exist as we could be running a random scene at any time.
// In a build, Create is called by the initialisation sequence on the first scene so it can be assumed to always exist
public class EntryPoint : MonoBehaviour
{
    private static EntryPoint s_Instance;

    public static EntryPoint Instance
    {
        get
        {
#if UNITY_EDITOR
            //we only test in editor, as this would be a check every access which is useless performance hit and in a build
            //this will be created by the startup sequence and set as DontDestroyOnLoad all through the game, so we are sure it's there
            if (s_Instance == null) Create();
#endif
            return s_Instance;
        }
    }

    /// <summary>
    /// This is called either by :
    /// - the Menu script that will create it at the game launch (in a full game, there could be a Loading scene loaded
    /// in first that will create it and display a progress bar possibly)
    /// - On the first call to Instance if it wasn't created (in editor only, see comment in that function)
    ///
    /// It take care of creating all globals data and initialize all system that need to be (like databases).
    /// </summary>
    public static void Create()
    {
        //in this sample we use the main menu scene as initialisation. That mean going back to main menu can trigger again
        //this initialization. So we check if we were already initialized. In a full project, this Create would be called
        //in a scene only run when the game launch, *before* main menu scene.
        if(s_Instance != null)
            return;

        s_Instance = Instantiate(Resources.Load<EntryPoint>("Main"));
        if (s_Instance == null)
        {
            Debug.LogError("Fatal Error, couldn't load the Main prefab from the Resources folder");
            return;
        }
        
        DontDestroyOnLoad(s_Instance);
        
        //This will init the Database and create the lookup dictionary matching an Item ScriptableObject and it's unique ID
        s_Instance.ItemDB.Init();
        
        s_Instance.m_PanelInstance = Instantiate(s_Instance.PanelPrefab);
        
        s_Instance.m_UI = Instantiate(s_Instance.UIPrefab);
        DontDestroyOnLoad(s_Instance.m_UI);
    }

    public static void StartPermanentCoroutine(IEnumerator coroutine)
    {
        s_Instance.StartCoroutine(coroutine);
    }

    public ItemDatabase ItemDB;
    public GameObject PlayerPrefab;

    [SerializeField] private GameObject UIPrefab;
    [SerializeField] private LoadingPanel PanelPrefab;
    
    public static LoadingPanel LoadingPanel => Instance.m_PanelInstance;

    private GameObject m_UI;
    private LoadingPanel m_PanelInstance;
}
