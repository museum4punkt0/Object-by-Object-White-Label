using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayerOfChildren
{
    public static void SetLayerAllChildren(Transform root, int layer)
    {
        foreach (Transform child in root)
        {
            child.gameObject.layer = layer;
            SetLayerAllChildren(child, layer);
        }
    }
}
