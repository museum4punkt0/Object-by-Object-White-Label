using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ThreeDManager : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Manipulation3D _manipulation3D = null;
	[SerializeField] private Transform _itemRoot = null;
	#endregion
	#region Private
	private Wezit.Poi m_PoiData;
	#endregion
	#endregion

	#region Properties
	public UnityEvent ItemManipulated = new();
	#endregion

	#region Methods
	#region Monobehaviours
	#endregion

	#region Public
	public async void Inflate(Wezit.Poi poi)
	{
		foreach(Transform child in _itemRoot)
        {
			Destroy(child.gameObject);
        }

		m_PoiData = poi;
		GameObject model = await Utils.GLTFSpawner.SpawnGLTF(_itemRoot, m_PoiData);
		_manipulation3D.SetFocusObject(model);
		_manipulation3D.FirstManipulated.RemoveAllListeners();
		_manipulation3D.FirstManipulated.AddListener(OnItemManipulated);
	}
	#endregion

	#region Private
	private void OnItemManipulated()
    {
		ItemManipulated?.Invoke();
    }
	#endregion
	#endregion
}
