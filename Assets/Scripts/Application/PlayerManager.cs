using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Wezit;

public class PlayerManager : Singleton<PlayerManager>
{
    public bool IsChallenge;
    public PlayerData Player;
    public Tour CurrentTour = null;
    public Poi CurrentPoi;
    public Poi CurrentContent;
    public string Language = "de";

    public static string PlayerDataPath
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    public static string SelfiesPath
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, "selfies");
#else
            return "file://" + Path.Combine(Application.persistentDataPath, "selfies");
#endif
        }
    }

    public static string SelfiesScreenshotPath
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, "selfies");
#else
            return "selfies";
#endif
        }
    }

    public void Init()
    {
        Player = new PlayerData();
        if(!Directory.Exists(SelfiesPath))
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(SelfiesPath);
#else
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SelfiesScreenshotPath));
#endif
        }
    }

    public void DeleteSave()
    {
        if(Directory.Exists(SelfiesPath))
        {
            Directory.Delete(SelfiesPath, true);
        }

        Player.Delete();
        Player = new PlayerData();
    }
}
