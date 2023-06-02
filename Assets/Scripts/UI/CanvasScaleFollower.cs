using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Follows the scale of the root canvas, not taking into account the possible scale changes of the hierarchy.
/// Example : pins on a scalable map.
/// </summary>
[RequireComponent(typeof(ScaleConstraint))]
public class CanvasScaleFollower : MonoBehaviour
{
	#region Fields
	// [SearchComponent]
	[SerializeField] private ScaleConstraint _scaleConstraint = null;
	private Transform _rootCanvasTransform = null;
	#endregion Fields

	#region Awake
	private void Start()
	{
		Canvas[] canvases = GetComponentsInParent<Canvas>();
		Canvas rootCanvas = null;

		foreach (Canvas canvas in canvases)
		{
			if (canvas.gameObject != gameObject)
			{
				rootCanvas = canvas;
			}
		}
		
		if (rootCanvas != null)
		{
			_rootCanvasTransform = rootCanvas.transform;
		}
		else
		{
			Debug.LogError("[" + GetType().ToString() + "] No canvas found for follower " + name);
		}

		if (_rootCanvasTransform != null && _scaleConstraint != null)
		{
			ConstraintSource newConstraintSource = new ConstraintSource();
			newConstraintSource.sourceTransform = _rootCanvasTransform;
			newConstraintSource.weight = 1;
			_scaleConstraint.AddSource(newConstraintSource);
		}
	}
	#endregion Awake
}
