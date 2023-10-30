using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;

public class ARItem3D : ARItemBase
{
    #region Fields
    #region SerializeField
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnMouseDown()
    {
        if(!ClickManager.ClickingOnUI)
        {
            ARItemClicked?.Invoke(m_Poi);
        }
    }
    #endregion
    #region Public
    public new async void Inflate(Poi poi)
    {
        base.Inflate(poi);
        GameObject model = await Utils.GLTFSpawner.SpawnGLTF(transform, m_Poi);

        if(!string.IsNullOrEmpty(poi.extent))
        {
            float.TryParse(poi.extent,
                           System.Globalization.NumberStyles.AllowDecimalPoint,
                           new System.Globalization.CultureInfo("en-US"),
                           out float size);
            model.transform.localScale = Vector3.one * size;
        }

        MeshRenderer childRenderer = model.GetComponentInChildren<MeshRenderer>();
        float maxSizeMagnitude = childRenderer.bounds.size.magnitude;

        MeshRenderer[] meshRenderers = model.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.SetFloat("metallicFactor", 0.3f);

            float meshSize = meshRenderer.bounds.size.magnitude;
            if (meshSize > maxSizeMagnitude)
            {
                maxSizeMagnitude = meshSize;
                childRenderer = meshRenderer;
            }
        }
        childRenderer.gameObject.AddComponent<BoxCollider>();
        childRenderer.gameObject.AddComponent<ColliderClicked>().Clicked.AddListener(OnChildColliderClicked);


        SetLayerOfChildren.SetLayerAllChildren(model.transform, 6);

    }
    #endregion
    #region Private
    private void OnChildColliderClicked()
    {
        ARItemClicked?.Invoke(m_Poi);
    }
    #endregion
    #endregion
}
