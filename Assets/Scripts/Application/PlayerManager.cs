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
            //return Path.Combine(Application.persistentDataPath, "playerData");
        }
    }

    public void Init()
    {
        Player = new PlayerData();
    }
}
