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
    [SerializeField] private InteractiveObjectRotator _objectRotator = null;
	[SerializeField] private Transform _itemRoot = null;
	#endregion
	#region Private
	private Wezit.Poi m_poiData;
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
			if(child.TryGetComponent<Light>(out Light light))
            {
				continue;
            }

			Destroy(child.gameObject);
        }

		m_poiData = poi;
		GameObject model = await Utils.GLTFSpawner.SpawnGLTF(_itemRoot, m_poiData);
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

        float size = Mathf.Max(sizeVector.x, sizeVector.y, sizeVector.z);

		_objectRotator.SetZoomLimits(6.013f / size * .4f, 6.013f / size * 1.7f);
		_objectRotator.Init();
		_objectRotator.ObjectHasBeenManipulated.RemoveAllListeners();
		_objectRotator.ObjectHasBeenManipulated.AddListener(OnItemManipulated);
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
