/**
 * Created by Willy
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;

public class ListPoiScript : MonoBehaviour
{
	#region Fields
	public static string TAG = "[ListPoiScript]";

	[SerializeField]
	private Image _imageBackground = null;
	[SerializeField]
	private Image _imageIndex = null;
	[SerializeField]
	private TextMeshProUGUI _textIndex = null;
	[SerializeField]
	private Image _imageComponent = null;
	[SerializeField]
	private TextMeshProUGUI _textTitle = null;
	private Button _btComponent = null;

	[SerializeField]
	private Wezit.Poi _wzPoi = null;
	public Wezit.Poi WzPoi { get => _wzPoi; }

	private string _poiId;
	public string PoiId { get => _poiId; }
	#endregion Fields

	#region Methods
	#region MonoBehavior
	private void Awake()
	{
		_btComponent = transform.GetComponent<Button>();
	}
	#endregion MonoBehavior

	#region Public
	public void InitWezitData(Wezit.Poi value)
	{
		_wzPoi = value;
		if (_wzPoi != null)
		{
			int poiIdInteger = 0;
			if (!String.IsNullOrEmpty(_wzPoi.identifier) && int.TryParse(_wzPoi.identifier, out poiIdInteger))
			{
				_poiId = _wzPoi.identifier;
			}
			else if (!String.IsNullOrEmpty(_wzPoi.subject) && int.TryParse(_wzPoi.subject, out poiIdInteger))
			{
				_poiId = _wzPoi.subject;
			}

			SetIndex(_poiId);
			SetImage(_wzPoi);
			SetTitle(_wzPoi.title.ToUpper());
		}
	}

	internal void SetBackgroundColor(Color color)
	{
		if (_imageBackground) _imageBackground.color = color;
	}

	internal void SetThemeColor(Color color)
	{
		if (_imageIndex) _imageIndex.color = color;
	}

	internal void SetIndex(string textContent)
	{
		if (_textIndex) _textIndex.text = StringUtils.CleanFromWezit(textContent);
	}

	internal void SetTitle(string textContent)
	{
		if (_textTitle) _textTitle.text = StringUtils.CleanFromWezit(textContent);
	}

	internal void SetTitleStyle(TextStyle textStyle)
	{
		if (_textIndex)
		{
			_textIndex.font = textStyle.fontLabelTitle;
			_textIndex.fontSize = textStyle.fontSizeLabelTitle;
			_textIndex.color = textStyle.fontColorLabelTitle;
		}

		if (_textTitle)
		{
			_textTitle.font = textStyle.fontLabelTitle;
			_textTitle.fontSize = textStyle.fontSizeLabelTitle;
			_textTitle.color = textStyle.fontColorLabelTitle;
		}
	}

	internal void SetImage(Wezit.Poi wezitPoi)
	{
		if (_imageComponent)
		{
			if (wezitPoi != null)
			{
				foreach (Wezit.Relation relation in wezitPoi.Relations)
				{
					if (relation.relation == Wezit.RelationName.REF_PICTURE)
					{
						StartCoroutine(ImageUtils.SetImage(
							_imageComponent, 
							relation.GetAssetSourceByTransformation(WezitSourceTransformation.thumbnail.ToString()),
							relation.GetAssetMimeTypeByTransformation(WezitSourceTransformation.thumbnail.ToString())));
						break;
					}
				}
			}
			else
			{
				StartCoroutine(ImageUtils.SetImage(_imageComponent, ""));
			}
		}
	}
	#endregion Public

	#region Private
	private void Update()
	{
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

		if (_btComponent != null)
		{
			_btComponent.onClick.AddListener(OnPoiClick);
		}
	}

	private void RemoveListeners()
	{
		if (_btComponent != null)
		{
			_btComponent.onClick.RemoveAllListeners();
		}
	}

	private void OnPoiClick()
	{
		if (_wzPoi != null) StoreAccessor.Dispatch(Store.SelectedPoi.ActionCreator.Set(_wzPoi));
	}
	#endregion Private

	#region Internals
	#endregion Internals
	#endregion Methods
}
