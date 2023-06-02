using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wezit;
using Utils;

public class MainPageView : BaseView
{
	#region Fields
	public static string WEZIT_TAG = "static_map";

	#region Serialize Fields
	[SerializeField] private Image _uIBackground = null;
	[SerializeField] private TextMeshProUGUI _titleText = null;
	[SerializeField] private TextMeshProUGUI _subtitleText = null;
	[SerializeField] private TextMeshProUGUI _descriptionText = null;
	[SerializeField] private ViewButton[] _viewButtons = null;
	[SerializeField] protected Button _backButton = null;
	[SerializeField] private bool _showChildrenPois = false;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	protected Wezit.Poi m_WzPoiData = null;
	protected KioskState m_NextState = KioskState.NONE;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.NONE;
	}

	public override void InitView()
	{
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		if (_uIBackground != null) _uIBackground.color = ColorsUtils.GetColorByHtmlString(AppConfig.Instance.ConfigModel.backgroundColor);

		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);

		AddListeners();

		base.ShowView();
	}

	public override void HideView()
	{
		RemoveListeners();
		base.HideView();
	}

	public override void OnLanguageUpdated(Language language)
	{
		if (m_IsActive && AppManager.Instance.loadingOver)
		{
			InitViewContentByLang(language);
		}
	}

	public override void OnSelectedPoi(Poi selectedPoi)
	{
		base.OnSelectedPoi(selectedPoi);

		if (_showChildrenPois && m_NextState != KioskState.NONE)
		{
			SetState(m_NextState);
		}
	}
	#endregion Public

	#region Internals
	protected virtual void ResetViewContent()
	{
		m_WzPoiData = null;
		if (_titleText) _titleText.text = "";
		if (_subtitleText) _subtitleText.text = "";
		if (_descriptionText) _descriptionText.text = "";

		if (_backButton)
		{
			_backButton.gameObject.SetActive(false);
		}

		foreach (ViewButton viewButton in _viewButtons)
		{
			viewButton.OnViewButtonClicked -= OnViewButtonClicked;
			viewButton.ResetButton();
		}

		m_NextState = KioskState.NONE;
	}

	protected virtual void InitViewContentByLang(Language language)
	{
		ResetViewContent();
		m_WzPoiData = WezitDataUtils.GetWezitPoiByTag(language, WEZIT_TAG);

		if (m_WzPoiData == null)
		{
			return;
		}

		if (_titleText && !string.IsNullOrEmpty(m_WzPoiData.title))
		{
			_titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.title.ToUpper());
		}
		if (_subtitleText && !string.IsNullOrEmpty(m_WzPoiData.subject))
		{
			_subtitleText.text = StringUtils.CleanFromWezit(m_WzPoiData.subject);
		}
		if (_descriptionText && !string.IsNullOrEmpty(m_WzPoiData.description))
		{
			_descriptionText.text = StringUtils.CleanFromWezit(m_WzPoiData.description);
		}

		if (_backButton)
		{
			_backButton.gameObject.SetActive(true);
		}

		if (_showChildrenPois == false)
		{
			foreach (ViewButton viewButton in _viewButtons)
			{
				viewButton.OnViewButtonClicked += OnViewButtonClicked;
				viewButton.InitWithLanguage(language);
			}
		}
		else
		{
			for (int i = 0; i < _viewButtons.Length; i++)
			{
				if (i < m_WzPoiData.childs.Count)
				{
					_viewButtons[i].OnViewButtonClicked += OnViewButtonClicked;
					_viewButtons[i].gameObject.SetActive(true);
					_viewButtons[i].InitWithPoi(m_WzPoiData.childs[i]);
				}
				else
				{
					_viewButtons[i].gameObject.SetActive(false);
				}
			}
		}
	}



	protected virtual void AddListeners()
	{
		RemoveListeners();

		if (_backButton) _backButton.onClick.AddListener(OnBackButton);
	}

	protected virtual void RemoveListeners()
	{
		if (_backButton) _backButton.onClick.RemoveAllListeners();
	}

	protected virtual KioskState GetReturnState()
	{
		return KioskState.NONE;
	}

	private void OnViewButtonClicked(KioskState newState, Wezit.Poi selectedPoi)
	{
		if (_showChildrenPois)
		{
			if (selectedPoi != null)
			{
				m_NextState = newState;
				StoreAccessor.Dispatch(Store.SelectedPoi.ActionCreator.Set(selectedPoi));
				return;
			}
		}

		SetState(newState);
	}

	private void OnBackButton()
	{
		AppManager.Instance.UnselectPoi();
		SetState(GetReturnState());
	}
	#endregion Internals
	#endregion Methods
}
