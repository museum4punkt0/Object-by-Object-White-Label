using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashView : Singleton<SplashView>
{
    #region Fields
    #region SerializeFields
    [SerializeField] private GameObject _root;
    [SerializeField] private RawImage _background;
    [SerializeField] private MenuManager _menuManager;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    private new void Awake()
    {
        base.Awake();
        _root.SetActive(true);
    }

    private void Start()
    {
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Hidden);
    }
    #endregion
    #region Public
    public void Hide()
    {
        _root.SetActive(false);
        AppManager.Instance.GoToState(KioskState.LANGUAGE_SELECTION);
    }
    #endregion
    #region Private

    #endregion
    #endregion
}
