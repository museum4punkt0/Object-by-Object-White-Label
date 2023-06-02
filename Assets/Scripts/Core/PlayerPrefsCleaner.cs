using UnityEngine;

public class PlayerPrefsCleaner : MonoBehaviour
{
	#region Fields
	[SerializeField] private bool _deleteResPlayerPrefsOnAppQuit = true;
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods

	private void OnApplicationQuit()
	{
		if (_deleteResPlayerPrefsOnAppQuit)
		{
			PlayerPrefs.DeleteKey("Screenmanager Resolution Height");
			PlayerPrefs.DeleteKey("Screenmanager Resolution Width");
		}
	}
	#endregion Methods
}