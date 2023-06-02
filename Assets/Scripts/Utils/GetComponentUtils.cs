/**
 * Created by Willy
 */

using UnityEngine;

public class GetComponentUtils
{
	public static T GetComponentInParents<T>(GameObject startObject) where T : Component
	{
		T returnObject = null;
		GameObject currentObject = startObject;
		while (!returnObject)
		{
			if (currentObject == currentObject.transform.root) return null;
			currentObject = currentObject.transform.parent.gameObject;
			returnObject = currentObject.GetComponent<T>();
		}
		return returnObject;
	}
}