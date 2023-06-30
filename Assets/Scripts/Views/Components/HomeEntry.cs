using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils;

[System.Serializable]
public class UnityEventHomeEntry : UnityEvent<Wezit.Poi> { };

public class HomeEntry : MonoBehaviour
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private TextMeshProUGUI _titleText = null;
	[SerializeField] private Image _imageBackground = null;
	[SerializeField] private Button _button = null;
	#endregion Serialize Fields

	#region Public Variables
	public UnityEventHomeEntry onHomeEntry;
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_WzPoiData;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	private void Awake()
	{
	}
	#endregion MonoBehaviour

	#region Public
	public void SetData(Wezit.Poi wzPoi)
	{
		ResetData();

		m_WzPoiData = wzPoi;
		_titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.title).ToUpper();
		if (_imageBackground != null) LoadImage(_imageBackground, m_WzPoiData, Wezit.RelationName.REF_PICTURE, WezitSourceTransformation.default_base, true);
	}

	public void ResetData()
	{
		m_WzPoiData = null;

		_titleText.text = "";
		if (_imageBackground != null) UnloadImage(_imageBackground);
	}
	#endregion Public

	#region Private
	private void LoadImage(Image imageComponent, Wezit.Node wzData, string targetRelation = "relationForShowPicture", string targetWzSourceTransformation = "original", bool fillParent = true, float crossFadeAlphaDuration = 0.25f)
	{
		if (imageComponent != null)
		{
			if (wzData != null)
			{
				foreach (Wezit.Relation relation in wzData.Relations)
				{
					if (relation.relation == targetRelation)
					{
						StartCoroutine(ImageUtils.SetImage(
							imageComponent, 
							relation.GetAssetSourceByTransformation(targetWzSourceTransformation),
							relation.GetAssetMimeTypeByTransformation(targetWzSourceTransformation),
							fillParent, 
							this, 
							crossFadeAlphaDuration));
						break;
					}
				}
			}
			else
			{
				UnloadImage(imageComponent);
			}
		}
	}

	private void UnloadImage(Image imageComponent)
	{
		if (imageComponent) ImageUtils.ResetImage(imageComponent);
	}

	private void OnEnable()
	{
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		RemoveListeners();

		if (_button != null)
		{
			_button.onClick.AddListener(OnButtonClick);
		}
	}

	private void RemoveListeners()
	{
		if (_button != null)
		{
			_button.onClick.RemoveAllListeners();
		}
	}

	private void OnButtonClick()
	{
		onHomeEntry?.Invoke(m_WzPoiData);
	}
	#endregion Private
	#endregion Methods
}
