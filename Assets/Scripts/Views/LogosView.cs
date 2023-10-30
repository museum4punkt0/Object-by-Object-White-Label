using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using SimpleJSON;

public class LogosView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Image _colorBG;
	[SerializeField] private CreditLogo _creditLogoPrefab;
	[SerializeField] private Transform _creditLogoRoot;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private string m_logosArraySettingKey = "template.spk.credits.logos.array";

	private int m_numberOfLogos;
	private int m_resizedLogos;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.RGPD;
	}

	public override void InitView()
	{
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);
		base.ShowView();
		AddListeners();
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

	#region Private
	private void AddListeners()
	{
		RemoveListeners();
	}

	private void RemoveListeners()
	{
	}

	private void ResetViewContent()
	{
        foreach (Transform child in _creditLogoRoot)
        {
            Destroy(child.gameObject);
        }

        m_numberOfLogos = 0;
		m_resizedLogos = 0;
	}

	private void InitViewContentByLang(Language language)
	{
        ResetViewContent();

		MenuManager.Instance.SetBackButtonState(ViewManager.Instance.PreviousKioskState);
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButtonLogo);

		JSONNode logoArray = Wezit.Settings.Instance.GetSettingArray(m_logosArraySettingKey, language);
        foreach (JSONNode logo in logoArray)
        {
			m_numberOfLogos++;

			string assetId = logo["image"].ToString()?.Replace("wzasset://", "");
			Wezit.WezitAssets.Asset asset = Wezit.AssetsLoader.GetAssetById(assetId.Replace("\"", ""));
            string source = "";
			if (asset != null)
			{
				source = asset.GetAssetSourceByTransformation("default");
			}

			CreditLogo instance = Instantiate(_creditLogoPrefab, _creditLogoRoot);
			instance.Inflate(
				source, 
				logo["url"],
				this);
			instance.LogoResized.AddListener(OnLogoResized);
        }
	}

	private void OnLogoResized()
    {
		m_resizedLogos++;
        if (m_resizedLogos == m_numberOfLogos)
        {
			StartCoroutine(LayoutGroupRebuilder.Rebuild(_creditLogoRoot.gameObject));
        }
    }
	#endregion Private
	#region Internals
	#endregion Internals
	#endregion Methods
}