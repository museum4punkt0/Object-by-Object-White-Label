using System.Collections;
using Lumpn.Matomo;
using UnityEngine;
using UnityEngine.Networking;

public class MatomoAnalyticsManager : Singleton<MatomoAnalyticsManager>
{
	private static string TAG = "<color=red>[MatomoAnalyticsManager]</color>";

	#region Const
	private const string TRACKER_DATA_PATH = "Analytics/MatomoTrackerData";
	#endregion Const

	#region Fields
	private MatomoTrackerData _trackerData = null;
	private MatomoTracker _tracker = null;
	private MatomoSession _currentSession = null;

	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	protected override void Awake()
	{
		base.Awake();

		if (AppManager.Instance.loadingOver)
		{
			InitTracker();
		}
		else
		{
			AppManager.Instance.onLoadingOver.AddListener(InitTracker);
		}
	}
	#endregion MonoBehaviour

	#region Internals
	private void InitTracker()
	{
		AppManager.Instance.onLoadingOver.RemoveListener(InitTracker);

		_trackerData = Resources.Load<MatomoTrackerData>(TRACKER_DATA_PATH);

		if (_trackerData == null)
		{
			Debug.LogError("Couldn't find Matomo Tracker Data at " + TRACKER_DATA_PATH);
			return;
		}

		_trackerData.MatomoUrl = AppConfig.Instance.ConfigModel.analyticsMatomoSettings.matomoUrl;
		_trackerData.WebsiteUrl = AppConfig.Instance.ConfigModel.analyticsMatomoSettings.websiteUrl;
		_trackerData.WebsiteId = int.Parse(AppConfig.Instance.ConfigModel.analyticsMatomoSettings.websiteId);

		Debug.Log(TAG + " InitTracker ---------------- ");
		Debug.Log(TAG + " MatomoUrl : " + _trackerData.MatomoUrl);
		Debug.Log(TAG + " WebsiteUrl : " + _trackerData.WebsiteUrl);
		Debug.Log(TAG + " WebsiteId : " + _trackerData.WebsiteId);

		_tracker = _trackerData.CreateTracker();
	}

	private IEnumerator RecordAction(string actionTitle, string actionUrl, float time)
	{
		UnityWebRequestAsyncOperation operation = _currentSession.Record(actionTitle, actionUrl, time);
		yield return operation;

		Debug.Log("Response for Matomo action " + actionTitle + " : " + operation.webRequest.responseCode);
	}
	#endregion Internals

	#region Public
	public void Init() { }

	public void StartNewPlayerSession()
	{
		if (_tracker == null)
		{
			Debug.LogError("Tracker not initialized !");
			return;
		}
		string playerId = System.Guid.NewGuid().ToString();
		_currentSession = _tracker.CreateSession(playerId);
	}

	public void RecordAppOpen()
	{
		StartNewPlayerSession();

		if (_currentSession == null)
		{
			Debug.LogError("Session not initialized !");
			return;
		}

		StartCoroutine(RecordAction("App Open", "", 0));
	}

	// public void RecordNewSessionStarted()
	// {
	// 	StartNewPlayerSession();

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("New Session", "", 0));
	// }

	public void RecordLanguageSession(string language)
	{
		if (_currentSession == null)
		{
			Debug.LogError("Session not initialized !");
			return;
		}

		StartCoroutine(RecordAction("Langue sélectionnée : " + language, "", 0));
	}

	public void RecordItemStepSelection(int stepId, Wezit.Poi stepWezitPoi, bool isRightChoice)
	{
		if (_currentSession == null)
		{
			Debug.LogError("Session not initialized !");
			return;
		}

		StartCoroutine(RecordAction("Etape n° " + stepId + " // Etape sélectionnée : " + stepWezitPoi.pid + " // Choix correct : " + (isRightChoice ? "OUI" : "NON"), stepWezitPoi.pid, 0));
	}

	// public void RecordItemVolcanoSelection(int pairId, Wezit.Poi volcanoWezitPoi)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Paire : " + pairId + " // Volcan sélectionné : " + volcanoWezitPoi.pid, volcanoWezitPoi.pid, 0));
	// }

	// public void RecordVolcanosComparison(bool sameVolcanoTypeFound)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Paire correcte : " + (sameVolcanoTypeFound ? "OUI" : "NON"), "", 0));
	// }

	// public void RecordScoreGame(uint nbPairFound)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Nombre de paires identifiées : " + nbPairFound, "", 0));
	// }

	public void RecordEndSession(float timeSession)
	{
		if (_currentSession == null)
		{
			Debug.LogError("Session not initialized !");
			return;
		}

		StartCoroutine(RecordAction("Durée de la session : " + (int)timeSession + " secondes", "", 0));
	}

	// public void RecordAppOpen()
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		StartNewPlayerSession();
	// 	}

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("App Open", "splashscreen", 0));
	// }

	// public void RecordHome()
	// {
	// 	// When going to home view, create new player.
	// 	StartNewPlayerSession();

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Home", "home", 0));
	// }

	// public void RecordStandby()
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Sleep", "sleep", 0));
	// }

	// public void RecordChooseLanguage(string language)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		StartNewPlayerSession();
	// 	}

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	StartCoroutine(RecordAction("Selection de Langue", "langselect/" + Uri.EscapeDataString(language), 0));
	// }

	// public void RecordTourView(string tourId, string tourTitle, string language)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		StartNewPlayerSession();
	// 	}

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	string pageUrl = string.Format("parcours/{0}/{1}/{2}", Uri.EscapeDataString(language), Uri.EscapeDataString(tourId), Uri.EscapeDataString(tourTitle));
	// 	StartCoroutine(RecordAction("Page Parcours", pageUrl, 0));
	// }

	// public void RecordPoiView(string poiId, string poiTitle, string language)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		StartNewPlayerSession();
	// 	}

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	string pageUrl = string.Format("poi/{0}/{1}/{2}", Uri.EscapeDataString(language), Uri.EscapeDataString(poiId), Uri.EscapeDataString(poiTitle));
	// 	StartCoroutine(RecordAction("Fiche Poi", pageUrl, 0));
	// }

	// public void RecordGlossary(string glossaryId, string glossaryTitle, string language)
	// {
	// 	if (_currentSession == null)
	// 	{
	// 		StartNewPlayerSession();
	// 	}

	// 	if (_currentSession == null)
	// 	{
	// 		Debug.LogError("Session not initialized !");
	// 		return;
	// 	}

	// 	string pageUrl = string.Format("glossary/{0}/{1}/{2}", Uri.EscapeDataString(language), Uri.EscapeDataString(glossaryId), Uri.EscapeDataString(glossaryTitle));
	// 	StartCoroutine(RecordAction("Glossaire", pageUrl, 0));
	// }
	#endregion Public
	#endregion Methods
}