using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Wezit;

[Serializable]
public class CurrentStateData
{
    [FormerlySerializedAs("Tour")]
    public Tour Tour = null;

    [FormerlySerializedAs("CurrentPoi")]
    public Poi CurrentPoi;

    [FormerlySerializedAs("CurrentContent")]
    public Poi CurrentContent;

    [FormerlySerializedAs("Language")]
    public string Language = "de";
}