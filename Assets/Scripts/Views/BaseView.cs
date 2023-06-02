using System;
using UnityEngine;
using Wezit;
using UnityEngine.Events;

/// <summary>
/// Base view for navigation in a Wezit-related app.
/// A view generally corresponds to a Wezit Poi.
/// </summary>
public abstract class BaseView : MonoBehaviour
{
	#region Fields
	#region Serialize Fields
	[SerializeField] protected GameObject _interfaceRoot = null;
	[Tooltip("Here you can setup actions when the view becomes visible (Fade In, etc)")]
	[SerializeField] protected UnityEvent _onInterfaceVisible = null;
	[Tooltip("Here you can setup actions when the view becomes hidden (Fade Out, etc)")]
	[SerializeField] protected UnityEvent _onInterfaceHidden = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private CanvasGroupFader m_Fader;
	protected IDisposable m_StoreSubscription;
	protected bool m_IsActive;
	private BaseView m_ViewToHideWhenFadeIn = null;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	public bool IsActive { get { return m_IsActive; } }
	internal CanvasGroupFader Fader { get => m_Fader; }
	#endregion Properties

	#region Methods
	#region Public
	public abstract KioskState GetKioskState();

	public void SetState(KioskState state)
	{
		Debug.Log(" BaseView - SetState - " + state);
		AppManager.Instance.GoToState(state);
	}

	public void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}

	public abstract void InitView();

	public virtual void ShowView()
	{
		SetInterfaceVisible(true);
	}

	public virtual void HideView()
	{
		SetInterfaceVisible(false);
	}

	public abstract void OnLanguageUpdated(Language language);

	public virtual void OnSelectedPoi(Poi selectedPoi) { }

	public void SetInterfaceVisible(bool visible)
	{
		m_IsActive = visible;
		m_ViewToHideWhenFadeIn = null;

		if (_interfaceRoot)
		{
			_interfaceRoot.SetActive(visible);

			if (visible == true)
			{
				if (_onInterfaceVisible != null)
				{
					_onInterfaceVisible.Invoke();
					m_ViewToHideWhenFadeIn = ViewManager.Instance.GetOldView();
					if (m_Fader)
					{
						m_Fader.StartFadingFromInit();
						ViewManager.Instance.PrepareHideOldView();
					}
					else
					{
						ViewManager.Instance.PrepareHideOldView();
						OnFadeEnd();
					}
				}
			}
			else
			{
				if (_onInterfaceHidden != null)
				{
					_onInterfaceHidden.Invoke();
				}
			}
		}
	}

	public virtual void PrepareHideView() { }
	#endregion Public

	#region MonoBehaviour
	protected virtual void Awake()
	{
		m_Fader = _interfaceRoot.GetComponentInChildren<CanvasGroupFader>();
		AppManager.Instance.onLoadingOver.AddListener(OnLoadingOver);
	}

	void Update()
	{
		UpdateView();
	}
	#endregion MonoBehaviour

	#region Private
	private void OnLoadingOver()
	{
		if (m_Fader)
		{
			if (AppConfig.Instance.ConfigModel.screenFadeTime > 0)
			{
				m_Fader.FadeTime = AppConfig.Instance.ConfigModel.screenFadeTime;
			}
			m_Fader.OnFadeEnd.AddListener(OnFadeEnd);
		}
	}

	private void OnFadeEnd()
	{
		OnFadeEndView();
		if (m_ViewToHideWhenFadeIn != null)
		{
			m_ViewToHideWhenFadeIn.HideView();
		}
		else
		{
			ViewManager.Instance.HideOldView();
		}
	}
	#endregion Private

	#region Internals

	protected virtual void UpdateView() { }
	protected virtual void OnFadeEndView() { }
	#endregion Internals
	#endregion Methods
}
