/**
 * Created by Willy
 */

using UnityEngine;
using UnityEngine.UI;

public class ImageDistributor : MonoBehaviour
{
	#region Fields
	public static string TAG = "<color=orange>[ImageDistributor]</color>";

	#region Serialize Fields
	[SerializeField] private Sprite[] _sprites = null;
	[SerializeField] private Image[] _fields = null;
	#endregion Serialize Fields

	#region Private m_Variables
	#endregion Private m_Variables
	#endregion Fields

	#region Methods
	#region MonoBehavior
	void Start()
	{
		DistributeSprites();
	}
	#endregion MonoBehavior

	#region Private
	private void DistributeSprites()
	{
		if (_sprites.Length == 0 || _fields.Length == 0) return;

		if (_sprites.Length != _fields.Length)
		{
			Debug.LogWarning(TAG + " DistributeSprites / both lists are not equals");
			return;
		}

		Utils.ArrayUtils.ShuffleArray(_sprites);
		Utils.ArrayUtils.ShuffleArray(_fields);

		for (int i = 0; i < _sprites.Length; i++)
			_fields[i].sprite = _sprites[i];
	}
	#endregion Private
	#endregion Methods
}
