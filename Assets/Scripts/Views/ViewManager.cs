﻿using System;
using UnityEngine;
using UniRx;
using Wezit;

/// <summary>
/// The ViewManager manages the different views integrated in a Wezit-related app.
/// </summary>
public class ViewManager : Singleton<ViewManager>
{
	#region Fields
	#region Serialize Fields
	[SerializeField] BaseView[] _views = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	protected IDisposable m_StoreSubscription;
	protected KioskState m_CurState = KioskState.NONE;
	protected KioskState m_OldState = KioskState.NONE;
	protected Language m_CurrentLanguage;
	protected Wezit.Poi m_SelectedPoi;
	protected BaseView m_CurrentView = null;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	public Language CurrentLanguage { get { return m_CurrentLanguage; } }
	public Wezit.Poi SelectedPoi { get { return m_SelectedPoi; } }
	public KioskState CurrentKioskState { get { return m_CurState; } }
	public KioskState PreviousKioskState { get { return m_OldState; } }
	#endregion Properties

	#region Methods
	#region Public
	public void PrepareHideOldView()
	{
		if (m_OldState != KioskState.NONE)
		{
			BaseView oldView = FindView(m_OldState);
			oldView.PrepareHideView();
			if (oldView.Fader) oldView.Fader.StopFading();
		}
	}

	public void HideOldView()
	{
		if (m_OldState != KioskState.NONE)
		{
			BaseView oldView = FindView(m_OldState);
			oldView.HideView();
		}
	}

	public BaseView GetOldView()
	{
		if (m_OldState != KioskState.NONE)
		{
			return FindView(m_OldState);
		}
		return null;
	}

	public void GoBack()
	{
		AppManager.Instance.GoToState(m_OldState);
	}
	#endregion Public

	#region Private
	private void Start()
	{
		m_CurrentLanguage = StoreAccessor.State.Language;

		m_StoreSubscription = StoreAccessor.Subject.Subscribe((state) =>
		{
			OnKioskStateChanged(state.KioskState);
			OnPoiChanged(state.SelectedPoi);
			OnLanguageChanged(state.Language);
		});

		InitViews();

		OnKioskStateChanged(StoreAccessor.State.KioskState);
	}

	private void InitViews()
	{
		foreach (BaseView view in _views)
		{
			if (view != null)
			{
				view.InitView();
			}
			else
			{
				Debug.LogError("[ViewManager] A view is null in the array.");
			}
		}
	}

	private void OnKioskStateChanged(KioskState newState)
	{
		BaseView newView = FindView(newState);
		if (newState != KioskState.NONE)
		{
			if (newState != m_CurState)
			{
				m_OldState = m_CurState;
				m_CurState = newState;


				m_CurrentView = newView;
				newView.ShowView();
			}
			else if (newView.IsActive == false)
			{
				newView.ShowView();
			}
		}
	}

	private void OnLanguageChanged(Language language)
	{
		if (language != m_CurrentLanguage)
		{
			m_CurrentLanguage = language;
			if (m_CurrentView) m_CurrentView.OnLanguageUpdated(language);
		}
	}

	private void OnPoiChanged(Poi selectedPoi)
	{
		if (m_SelectedPoi != selectedPoi)
		{
			m_SelectedPoi = selectedPoi;
			if (m_CurrentView) m_CurrentView.OnSelectedPoi(selectedPoi);
		}
	}

	private BaseView FindView(KioskState state)
	{
		foreach (BaseView view in _views)
		{
			if (view != null && view.GetKioskState() == state)
			{
				return view;
			}
		}

		return null;
	}
	#endregion Private
	#endregion Methods

}
