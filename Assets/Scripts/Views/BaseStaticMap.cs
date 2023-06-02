using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class BaseStaticMap : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Image _uIBackground = null;
	[SerializeField] private Image _imageMap = null;
	[Space]
	[SerializeField] private GameObject _poisListLayout = null;
	[Space]
	[SerializeField] protected TextMeshProUGUI _titleText = null;
	[SerializeField] protected TextMeshProUGUI _subtitleText = null;
	[SerializeField] protected Button _backButton = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	protected Wezit.Poi m_WzPoiData = null;
	protected Dictionary<string, GameObject> m_MapPoiDict;
	protected Dictionary<string, GameObject> m_ListPoiDict;
	private Coroutine m_SetImageMapRoutine = null;
	private Coroutine m_InitImageRoutine = null;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	protected string TAG { get { return "[" + GetType().ToString() + "]"; } }
	protected Image ImageMap { get { return _imageMap; } }
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.NONE;
	}

	public override void InitView()
	{
	}

	public override void ShowView()
	{
		if (_uIBackground != null) _uIBackground.color = ColorsUtils.GetColorByHtmlString(AppConfig.Instance.ConfigModel.backgroundColor);

		base.ShowView();

		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);
		AddListeners();
	}

	public override void HideView()
	{
		RemoveListeners();
		DestroyMapPois();
		DestroyListPois();

		base.HideView();
	}

	public override void PrepareHideView()
	{
		RemoveListeners();
		DestroyMapPois();
		DestroyListPois();

		base.PrepareHideView();
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

	#region Internals
	protected abstract void GeneratePois(Language language, Wezit.Poi wezitPoi);

	protected abstract string GetDataTag();

	protected abstract IEnumerator InitImageViewer(string assetSource);

	protected virtual void AddListeners()
	{
		RemoveListeners();

		if (_backButton) _backButton.onClick.AddListener(OnBackButton);
	}

	protected virtual void RemoveListeners()
	{
		if (_backButton) _backButton.onClick.RemoveAllListeners();
	}

	protected virtual void SetData(Wezit.Poi data)
	{
		m_WzPoiData = data;
	}

	protected virtual void ResetViewContent()
	{
		SetData(null);
		if (_imageMap) StartCoroutine(ImageUtils.SetImage(_imageMap, ""));
		if (_titleText) _titleText.text = "";
		if (_subtitleText) _subtitleText.text = "";

		if (_backButton)
		{
			_backButton.gameObject.SetActive(false);
		}

		DestroyMapPois();
		DestroyListPois();
	}

	protected virtual void InitViewContentByLang(Language language)
	{
		ResetViewContent();
		SetData(WezitDataUtils.GetWezitPoiByTag(language, GetDataTag()));
		if (m_WzPoiData == null) return;

		StartCoroutine(LoadMapAndPois(language, m_WzPoiData));

		SetupTexts();

		if (_backButton)
		{
			_backButton.gameObject.SetActive(true);
		}
	}

	protected virtual KioskState GetReturnState()
	{
		return KioskState.NONE;
	}

	protected virtual void SetupTexts()
	{
		if (_titleText) _titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.title).ToUpper();
		if (_subtitleText) _subtitleText.text = StringUtils.CleanFromWezit(m_WzPoiData.description);
	}

	protected IEnumerator LoadMapAndPois(Language language, Wezit.Poi wezitPoi)
	{
		if (m_SetImageMapRoutine != null) StopCoroutine(m_SetImageMapRoutine);

		m_SetImageMapRoutine = StartCoroutine(SetImageMap(wezitPoi));
		yield return m_SetImageMapRoutine;

		GeneratePois(language, wezitPoi);
	}

	private IEnumerator SetImageMap(Wezit.Poi wezitPoi)
	{
		if (m_InitImageRoutine != null) StopCoroutine(m_InitImageRoutine);

		if (_imageMap == null) yield break;

		if (wezitPoi != null)
		{
			string assetSource = "";
			foreach (Wezit.Relation relation in wezitPoi.Relations)
			{
				if (relation.relation == Wezit.RelationName.SHOW_PICTURE)
				{
					assetSource = relation.GetAssetSourceByTransformation(WezitSourceTransformation.original.ToString());
					if (!String.IsNullOrEmpty(assetSource))
					{
						m_InitImageRoutine = StartCoroutine(InitImageViewer(assetSource));
						yield return m_InitImageRoutine;
					}
					else
					{
						yield return null;
					}
					break;
				}
			}
		}
		else
		{
			yield return ImageUtils.SetImage(_imageMap, "");
		}
	}

	protected virtual void CreateMapPoi(MapPoiScript mapPoiPrefab, Wezit.Poi wzPoi, Color pinColor, Rect mapImageRect, TextStyle textStyle = null, int positionIndex = 0, string specialId = "")
	{
		MapPoiScript mapPoiScriptInstance = Instantiate(mapPoiPrefab, _imageMap.gameObject.transform);
		mapPoiScriptInstance.InitWezitData(wzPoi, positionIndex, wzPoi.type.Split('|'));
		mapPoiScriptInstance.SetPinColor(pinColor);

		Vector2 imageMapPivot = _imageMap.rectTransform.pivot;

		if (textStyle != null)
		{
			mapPoiScriptInstance.SetTextStyle(textStyle);
		}

		if (mapPoiScriptInstance.PoiPosition != Vector2.positiveInfinity)
		{
			float xPosition = Utils.MathUtils.Map(mapPoiScriptInstance.PoiPosition.x, 0, 1, -mapImageRect.width / 2, mapImageRect.width / 2) + (0.5f - imageMapPivot.x) * mapImageRect.width;
			float yPosition = Utils.MathUtils.Map(mapPoiScriptInstance.PoiPosition.y, 0, 1, mapImageRect.height / 2, -mapImageRect.height / 2) + (0.5f - imageMapPivot.y) * mapImageRect.height;
			mapPoiScriptInstance.transform.localPosition = new Vector2(xPosition, yPosition);
		}
		else
		{
			Debug.LogError(TAG + "This poi needs location : " + mapPoiScriptInstance.WzPoi.pid);
		}

		m_MapPoiDict.Add(string.IsNullOrEmpty(specialId) ? mapPoiScriptInstance.WzPoi.pid : (specialId + "_" + positionIndex), mapPoiScriptInstance.gameObject);
	}

	protected void CreateListPoi(ListPoiScript listPoiPrefab, Wezit.Poi wzPoi, Color poiColor, TextStyle textStyle = null)
	{
		ListPoiScript listPoiScriptInstance = Instantiate(listPoiPrefab, _poisListLayout.transform);
		listPoiScriptInstance.InitWezitData(wzPoi);
		listPoiScriptInstance.SetBackgroundColor(_poisListLayout.GetComponent<Image>().color);
		listPoiScriptInstance.SetThemeColor(poiColor);

		if (textStyle != null)
		{
			listPoiScriptInstance.SetTitleStyle(textStyle);
		}

		m_ListPoiDict.Add(listPoiScriptInstance.PoiId, listPoiScriptInstance.gameObject);
	}

	protected virtual void DestroyMapPois()
	{
		if (m_MapPoiDict != null)
		{
			foreach (string key in m_MapPoiDict.Keys)
			{
				Destroy(m_MapPoiDict[key]);
			}

			m_MapPoiDict.Clear();
		}
	}

	private void DestroyListPois()
	{
		if (m_ListPoiDict != null)
		{
			foreach (string key in m_ListPoiDict.Keys)
			{
				Destroy(m_ListPoiDict[key]);
			}

			m_ListPoiDict.Clear();
		}
	}

	private void OnBackButton()
	{
		AppManager.Instance.UnselectPoi();
		AppManager.Instance.GoToState(GetReturnState());
	}
	#endregion Internals
	#endregion Methods
}
