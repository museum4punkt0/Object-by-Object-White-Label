using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ViewButton : MonoBehaviour
{
	#region Fields
	[SerializeField] private KioskState _viewState = KioskState.NONE;
	[SerializeField] private Button _button = null;
	[SerializeField] private Image _image = null;
	[SerializeField] private TextMeshProUGUI _viewTitleText = null;
	[SerializeField] private TextMeshProUGUI _viewSubtitleText = null;

	private event Action<KioskState, Wezit.Poi> _onViewButtonClicked = null;
	private bool _waitingForActivation = false;
	private Wezit.Poi _wzPoiData = null;
	#endregion Fields

	#region Properties
	public event Action<KioskState, Wezit.Poi> OnViewButtonClicked
	{
		add { _onViewButtonClicked -= value; _onViewButtonClicked += value; }
		remove { _onViewButtonClicked -= value; }
	}
	#endregion Properties

	#region Methods
	public void InitWithLanguage(Language language)
	{
		string tag = _viewState.ToString().ToLower();

		if (string.IsNullOrEmpty(tag) == false)
		{
			_wzPoiData = WezitDataUtils.GetWezitPoiByTag(language, tag);

			if (_viewTitleText != null)
			{
				if (!String.IsNullOrEmpty(_wzPoiData.subject) && _viewSubtitleText == null)
				{
					_viewTitleText.text = StringUtils.CleanFromWezit(_wzPoiData.subject).ToUpper();
				}
				else
				{
					_viewTitleText.text = StringUtils.CleanFromWezit(_wzPoiData.title).ToUpper();
				}
			}

			if (_viewSubtitleText != null && !String.IsNullOrEmpty(_wzPoiData.subject))
			{
				_viewSubtitleText.text = StringUtils.CleanFromWezit(_wzPoiData.subject).ToUpper();
			}

			if (isActiveAndEnabled)
			{
				StartCoroutine(InitImage(_wzPoiData));
			}
			else
			{
				_waitingForActivation = true;
			}
		}
	}

	public void InitWithPoi(Wezit.Poi wzPoi)
	{
		_wzPoiData = wzPoi;

		if (_viewTitleText != null)
		{
			if (!String.IsNullOrEmpty(_wzPoiData.subject) && _viewSubtitleText == null)
			{
				_viewTitleText.text = StringUtils.CleanFromWezit(_wzPoiData.subject).ToUpper();
			}
			else
			{
				_viewTitleText.text = StringUtils.CleanFromWezit(_wzPoiData.title).ToUpper();
			}
		}

		if (_viewSubtitleText != null && !String.IsNullOrEmpty(_wzPoiData.subject))
		{
			_viewSubtitleText.text = StringUtils.CleanFromWezit(_wzPoiData.subject).ToUpper();
		}

		if (isActiveAndEnabled)
		{
			StartCoroutine(InitImage(_wzPoiData));
		}
		else
		{
			_waitingForActivation = true;
		}
	}

	public void ResetButton()
	{
		StopAllCoroutines();
	}

	private void OnEnable()
	{
		AddListeners();

		if (_waitingForActivation)
		{
			_waitingForActivation = false;
			StartCoroutine(InitImage(_wzPoiData));
		}
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		if (_button != null)
		{
			_button.onClick.AddListener(OnButtonClicked);
		}
	}

	private void RemoveListeners()
	{
		if (_button != null)
		{
			_button.onClick.RemoveListener(OnButtonClicked);
		}
	}

	private void OnButtonClicked()
	{
		_onViewButtonClicked?.Invoke(_viewState, _wzPoiData);
	}

	private IEnumerator InitImage(Wezit.Poi wezitPoi)
	{
		if (_image == null) yield break;

		_image.enabled = false;

		if (wezitPoi != null)
		{
			foreach (Wezit.Relation relation in wezitPoi.Relations)
			{
				if (relation.relation == Wezit.RelationName.REF_PICTURE)
				{
					yield return ImageUtils.SetImage(_image, relation.GetAssetSourceByTransformation(WezitSourceTransformation.default2kpx.ToString()));
					_image.enabled = true;
					break;
				}

				if (relation.relation == Wezit.RelationName.SHOW_PICTURE)
				{
					yield return ImageUtils.SetImage(_image, relation.GetAssetSourceByTransformation(WezitSourceTransformation.default2kpx.ToString()));
					_image.enabled = true;
					break;
				}
			}
		}
		else
		{
			yield return ImageUtils.SetImage(_image, "");
		}
	}
	#endregion Methods
}
