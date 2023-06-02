using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// NOTE : This class is never serialized, so it's usefull to store data only on runtime !
// NEVER use it to store data locally or cloudly !!
public class PlayerRuntime
{
    public bool IsARCompatible = false;

    public CurrentStateData State = new CurrentStateData();
}