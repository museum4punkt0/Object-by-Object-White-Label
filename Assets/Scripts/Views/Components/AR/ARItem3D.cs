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
        MeshRenderer childRenderer = model.GetComponentInChildren<MeshRenderer>();
        childRenderer.gameObject.AddComponent<BoxCollider>();
        childRenderer.gameObject.AddComponent<ColliderClicked>().Clicked.AddListener(OnChildColliderClicked);

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
