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
		SetLayerOfChildren.SetLayerAllChildren(model.transform, 7);

		MeshRenderer[] meshRenderers = model.GetComponentsInChildren<MeshRenderer>();
		Vector3 sizeVector = Vector3.negativeInfinity;

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
			meshRenderer.material.SetFloat("metallicFactor", 0.3f);
			sizeVector.x = Mathf.Max(sizeVector.x, meshRenderer.bounds.size.x);
			sizeVector.y = Mathf.Max(sizeVector.y, meshRenderer.bounds.size.y);
			sizeVector.z = Mathf.Max(sizeVector.z, meshRenderer.bounds.size.z);
        }

        float size = Mathf.Max((sizeVector.x), (sizeVector.y), (sizeVector.z));

		_manipulation3D.SetZoomMinMax(size / 3.375f * -15, size / 3.375f * -3);
		_manipulation3D.SetFocusObject(model);
		_manipulation3D.Init();
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
