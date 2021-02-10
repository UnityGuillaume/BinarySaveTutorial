using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handle everything related to level loading and saving.
/// Also got Save/Get flag function that will append the level unique name to the flag before writing it. This can allow
/// to have similar object in each level but they all set flag based on which level they are on (like collectibles for example).
/// </summary>
public class LevelSystem : MonoBehaviour
{
    private static LevelSystem s_Instance;
    public static LevelSystem Instance => s_Instance;

    
    private static bool s_InTransition = false;
    public static bool InTransition => s_InTransition;
    
    private static bool s_IsLoading = false;
    
    public string UniqueName;
    private bool m_Started = false;

    private void Awake()
    {
        s_Instance = this;
    }

    private void Start()
    {
        m_Started = true;

        //spawning the player to the current active spawner
        var sp = SpawnPoint.GetActiveSpawnPoint(!s_IsLoading);
        if (sp != null)
        {
            PlayerSystem.SpawnAt(sp.transform);
        }

        s_IsLoading = false;
    }

    public void SaveFlag(string key, bool flag)
    {
        SaveSystem.SetFlag(UniqueName + "_" + key, flag);
    }

    public bool GetFlag(string key)
    {
        return SaveSystem.GetFlag(UniqueName + "_" + key);
    }

    public static void SaveData(BinaryWriter writer)
    {
        writer.Write(SceneManager.GetActiveScene().buildIndex);    
    }

    public static void LoadData(BinaryReader reader)
    {
        int level = reader.ReadInt32();
        LoadInScene(level, null, true);
    }
    
    //A static function that help to load into a scene number from anywhere, at given spawn point.
    //This will trigger a fade in and out of the LoadingPanel and will start a LoadScene when the fading is done
    public static void LoadInScene(int scene, string spawnPoint, bool isLoading = false)
    {
        //that flag is used to help debug : in editor we can press play in a scene without having gone through the
        //main menu. That mean we can reach a point where you load into a scene with both no player data loaded
        //and no spawn point defined. In that case the spawnPoint set as default will be used to spawn the player
        //allowing to play in editor without loading a save first.
        s_IsLoading = isLoading;
        
        SpawnPoint.SetTargetSpawn(spawnPoint);
        s_InTransition = true;
        EntryPoint.LoadingPanel.Fade(1.0f, () =>
        {
            SceneManager.LoadSceneAsync(scene).completed += operation =>
            {
                s_InTransition = false;
            };
        });
    }
}
