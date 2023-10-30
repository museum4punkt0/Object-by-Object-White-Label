using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;

public class ARItemRoot : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private ARItemSprite _spriteItemPrefab;
    [SerializeField] private ARItem3D _3DItemPrefab;
    [SerializeField] private Transform _itemsRoot;
    #endregion
    #region Private
    private Poi m_PoiData;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Poi> ARItemClicked = new();
    #endregion

    #region Methods
    #region Monobehaviour
    #endregion
    #region Public
    public void Inflate(Poi poi, Camera aRCamera)
    {
        m_PoiData = poi;

        foreach (Poi child in m_PoiData.childs)
        {
            if(child.type == ARItemTypes.threeD)
            {
                ARItem3D instance = Instantiate(_3DItemPrefab, _itemsRoot);
                instance.Inflate(child);
                instance.ARItemClicked.AddListener(OnARItemClicked);
            }
            else
            {
                ARItemSprite instance = Instantiate(_spriteItemPrefab, _itemsRoot);
                instance.Inflate(child, aRCamera);
                instance.ARItemClicked.AddListener(OnARItemClicked);
            }
        }
    }

    public void Toggle(bool isOn)
    {
        _itemsRoot.gameObject.SetActive(isOn);
    }
    #endregion
    #region Private
    private void OnARItemClicked(Poi poi)
    {
        ARItemClicked?.Invoke(poi);
    }
    #endregion
    #endregion
}