using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using Random = System.Random;

/// <summary>
/// This is the central point of everything save related. It is a system, so there is only one existing and it exist
/// as soon as the game starts.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class SaveHeaderData
    {
        public string Path;
        public string Time;
        public Texture2D Texture;
        public int Version;
    }
    
    //This need to be bumped when you change anything in the number or order of thing
    //serialized by the save system, this allow to handle older save in newer version!
    //See the CharacterControl.cs Load function for how it can be used for loading of older save.
    private static int VERSION = 2;
    private static string SaveGameLocation;

    public static SaveHeaderData SaveData => s_SaveData;

    private static SaveHeaderData s_SaveData;
    private static BinaryReader s_Reader;
    
    private static Dictionary<string, bool> s_Flags;
    
    private static Texture2D[] s_TexturePool;

    static SaveSystem()
    {
        SaveGameLocation = Application.persistentDataPath + "/saves/";
        
        if (!Directory.Exists(SaveGameLocation))
            Directory.CreateDirectory(SaveGameLocation);
        
        s_TexturePool = new Texture2D[8];
        
        Reset();
    }

    public static void Reset()
    {
        s_Flags = new Dictionary<string, bool>();
    }

    public static List<SaveHeaderData> GetSaveInfo()
    {
        //find all existing savefile.
        //for the sake of simplicity we do that each time and don't cache it as we want updated save file info
        //a more complete way would store latest cached edit time and update only if new edit time is more recent
        List<SaveHeaderData> saveData = new List<SaveHeaderData>();
        
        for(int i = 0; i < 8; ++i)
        {
            string f = SaveGameLocation + $"SaveGame{i}.usg";

            if (!File.Exists(f))
            {
                saveData.Add(null);
                continue;
            }

            BinaryReader reader = new BinaryReader(new FileStream(f, FileMode.Open));

            string date = reader.ReadString();
            int version = reader.ReadInt32();

            int count = reader.ReadInt32();
            byte[] texImg = reader.ReadBytes(count);
            
            reader.Close();
            
            if(s_TexturePool[i] == null)
                s_TexturePool[i] = new Texture2D(ScreenshotTaker.WIDTH, ScreenshotTaker.HEIGHT, DefaultFormat.LDR, TextureCreationFlags.None);
            
            s_TexturePool[i].LoadImage(texImg);

            saveData.Add(new SaveHeaderData()
            {
                Path = f,
                Time = date,
                Version = version,
                Texture = s_TexturePool[i]
            });
        }

        return saveData;
    }

    public static void SetFlag(string key, bool value)
    {
        s_Flags[key] = value;
    }

    public static bool GetFlag(string key)
    {
        if (!s_Flags.TryGetValue(key, out var flag))
            return false;

        return flag;
    }

    public static IEnumerator Load(string saveName)
    {
        string location = SaveGameLocation + saveName;

        if (!File.Exists(location))
        {
            Debug.LogError("Non existing save file");
            yield break;
        }
        
        //we copy our file into memory. As we will wait for some level loading to happen before continuing, we may have
        //case where if the game where to crash/forced to quit mid load, it would leave an open file.
        MemoryStream memStream = new MemoryStream();
        using (FileStream fileStream = File.OpenRead(location))
        {
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
        }

        s_Reader = new BinaryReader(memStream);
        
        //we are storing the loaded save file header, may be useful for other system loading, especially the version
        s_SaveData = new SaveHeaderData();
        
        s_SaveData.Time = s_Reader.ReadString();
        s_SaveData.Version = s_Reader.ReadInt32();
        
        //jump over the texture data as we don't need to read it when loading the game, just when getting the save info.
        int offset = s_Reader.ReadInt32();
        s_Reader.BaseStream.Seek(offset, SeekOrigin.Current);
        
        //read the flags
        s_Flags.Clear();
        int count = s_Reader.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            s_Flags.Add(s_Reader.ReadString(), s_Reader.ReadBoolean());
        }
        
        //read the level we need to load
        LevelSystem.LoadData(s_Reader);
        
        //wait till the level have finished loading so loading the player will have access to the navmesh and other
        //setting of the needed level
        while (LevelSystem.InTransition)
        {
            yield return null;
        }

        //read the player data. We don't have to wait for the level to be loaded (this will take multiple frames as we fade to black first)
        PlayerSystem.LoadData(s_Reader);
        
        s_Reader.Close();
    }
    

    public static void Save(string saveName, Texture2D screenshot)
    {
        string location = SaveGameLocation + saveName;

        BinaryWriter writer = new BinaryWriter(new FileStream(location, FileMode.Create));
        
        //record the time
        writer.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm"));
        //then the save file version
        writer.Write(VERSION);
        //then save the texture
        var bytes = ScreenshotTaker.SaveScreen();
        writer.Write(bytes.Length);
        writer.Write(bytes);
        
        //save the flags of the world
        writer.Write(s_Flags.Count);
        foreach (var p in s_Flags)
        {
            writer.Write(p.Key);
            writer.Write(p.Value);
        }

        //save the level we are currently in
        LevelSystem.SaveData(writer);

        //then save the character data
        PlayerSystem.SaveData(writer);

        writer.Close();
    }
}
