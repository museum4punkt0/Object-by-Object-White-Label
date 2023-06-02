using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
using Utils;

public class BaseDetailsView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Image _uIBackground = null;
	[Space]
	[SerializeField] protected TextMeshProUGUI _titleText = null;
	[SerializeField] protected TextMeshProUGUI _subtitleText = null;
	[SerializeField] protected TextMeshProUGUI _descriptionText = null;
	[SerializeField] protected Button _backButton = null;
	//[Space]
	//[SerializeField] protected HorizontalScrollSnap _scrollSnap = null;
	[SerializeField] private GameObject _scrollSnapPrefab = null;
	[SerializeField] private GameObject _scrollSnapPaginationPrefab = null;
	[SerializeField] private GameObject _paginationLayout = null;
	[SerializeField] private bool _mediaFillParent = false;
	[SerializeField] private bool _useParentDataForSubtitle = true;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	protected Wezit.Poi m_WzPoiData = null;
	protected Wezit.Poi m_WzPoiParentData = null;
	private List<Wezit.Relation> m_MediaList = null;
	private List<GameObject> m_MarkerList = null;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	protected virtual string TAG
	{
		get { return "<color=yellow>[" + GetType().ToString() + "]</color>"; }
	}
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	protected override void Awake()
	{
		base.Awake();
		m_MediaList = new List<Wezit.Relation>();
		m_MarkerList = new List<GameObject>();
	}
	#endregion MonoBehaviour

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

	public void Dispose()
	{
		RemoveListeners();
	}
	#endregion Public

	#region MonoBehavior
	#endregion MonoBehavior

	#region Internals
	protected override void OnFadeEndView()
	{

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

		CleanMediaPages();
	}

	protected virtual async UniTask InitViewContentByLang(Language language)
	{
		RemoveListeners();
		ResetViewContent();

		m_WzPoiData = ViewManager.Instance.SelectedPoi;

		if (m_WzPoiData.language != language.ToString())
		{
			m_WzPoiData = await GetCorrespondingPoiByLanguage(m_WzPoiData, language);
		}

		if (m_WzPoiData == null)
		{
			Debug.LogWarning(TAG + "Poi is null for lang : " + language.ToString());
		}
		else
		{
			if (string.IsNullOrEmpty(m_WzPoiData.parentPid))
			{
				m_WzPoiParentData = PoiStore.GetParentPoiByChildId(m_WzPoiData.pid);
			}
			else
			{
				m_WzPoiParentData = PoiStore.GetPoiById(m_WzPoiData.parentPid);
			}

			if (_titleText) _titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.title).ToUpper();

			if (m_WzPoiParentData != null && _useParentDataForSubtitle)
			{
				if (_subtitleText) _subtitleText.text = StringUtils.CleanFromWezit(m_WzPoiParentData.title).ToUpper();
			}
			else
			{
				if (_subtitleText) _subtitleText.text = StringUtils.CleanFromWezit(m_WzPoiData.subject).ToUpper();
			}

			if (_descriptionText) _descriptionText.text = StringUtils.CleanFromWezit(m_WzPoiData.description);

			if (_backButton)
			{
				_backButton.gameObject.SetActive(true);
			}

			StartCoroutine(InitDiapo());
		}

		AddListeners();
	}
	#endregion Internals

	#region Private
	private void OnBackButton()
	{
		AppManager.Instance.UnselectPoi();
		SetState(GetReturnState());
	}

	protected virtual IEnumerator InitDiapo()
	{
		CleanMediaPages();

		foreach (Wezit.Relation relation in m_WzPoiData.Relations)
		{
			if (relation.relation == Wezit.RelationName.SHOW_PICTURE) m_MediaList.Add(relation);
		}

		if (m_MediaList != null && m_MediaList.Count > 0)
		{
			yield return LoadMediaPages(m_MediaList);
			//if (_scrollSnap)
			//{
			//	_scrollSnap.OnCurrentScreenChange(_scrollSnap.CurrentPage);
			//	Utils.LayoutGroupRebuilder.Rebuild(_scrollSnap.gameObject); // reset RectMask2D 
			//}
		}
	}

	private IEnumerator LoadMediaPages(List<Wezit.Relation> mediaList)
	{
		if (_scrollSnapPrefab != null)
		{
			foreach (Wezit.Relation media in mediaList)
			{
				if (_scrollSnapPaginationPrefab != null)
				{
					GameObject marker = Instantiate(_scrollSnapPaginationPrefab) as GameObject;
					marker.transform.SetParent(_paginationLayout.transform);
					m_MarkerList.Add(marker);
				}

				GameObject page = Instantiate(_scrollSnapPrefab) as GameObject;
				yield return null;
				//if (_scrollSnap != null) _scrollSnap.AddChild(page);

				Image pageImage = page.GetComponentInChildren<Image>();

				pageImage.enabled = false;

				yield return ImageUtils.SetImage(
					pageImage, 
					media.GetAssetSourceByTransformation(WezitSourceTransformation.default_base),
					media.GetAssetMimeTypeByTransformation(WezitSourceTransformation.default_base),
					_mediaFillParent);

				pageImage.enabled = true;
			}
		}

		//if (_scrollSnap.ChildObjects != null && _scrollSnap.ChildObjects.Length > 0) _scrollSnap.ChangePage(0);
	}

	private void CleanMediaPages()
	{
		if (m_MediaList != null) m_MediaList.Clear();

		//if (_scrollSnap.ChildObjects != null && _scrollSnap.ChildObjects.Length > 0)
		//{
		//	GameObject[] childrenRemoved;
		//	_scrollSnap.RemoveAllChildren(out childrenRemoved);

		//	foreach (GameObject item in childrenRemoved)
		//	{
		//		Destroy(item);
		//	}
		//}

		if (m_MarkerList != null && m_MarkerList.Count > 0)
		{
			foreach (GameObject marker in m_MarkerList)
			{
				Destroy(marker);
			}
			m_MarkerList.Clear();
		}
	}

	protected async UniTask<Wezit.Poi> GetCorrespondingPoiByLanguage(Wezit.Poi currentPoi, Language language)
	{
		List<Wezit.Poi> resultData = await Wezit.StoreInitializer.Instance.GetPoiVersions(currentPoi.pid);
		if (resultData != null)
		{
			Wezit.Poi newPoi = resultData.Find(poi => poi.language == language.ToString());

			if (newPoi != null)
			{
				// Get the stored poi.
				return PoiStore.GetPoiById(newPoi.pid);
			}
		}

		Debug.LogError("Could not find corresponding poi of poi : " + currentPoi.pid + "for language : " + language.ToString());
		return null;
	}
	#endregion Private
	#endregion Methods
}
