using UnityEngine;

public class FullscreenSetter : MonoBehaviour
{
	#region Fields
    [SerializeField] private bool _fullscreen = false;
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
    private void Awake() 
    {
        Screen.fullScreen = _fullscreen;
#if UNITY_ANDROID && !UNITY_EDITOR
         Screen.SetResolution ((int)Screen.width, (int)Screen.height, true);
#endif

	}
#endregion Methods
}