using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContrastButton : MonoBehaviour
{
	#region Fields
	internal Button m_Button;
	[SerializeField] private Image _icon;
	[SerializeField] private ContrastPanel _contrastPanelPrefab = null;
	
	private Transform m_ContrastPanelRoot = null;
	private string m_title = null;
	private string[] m_paragraphs = null;
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	internal void Awake()
	{
		m_Button = GetComponent<Button>();
		SetBackgroundColor();
		AddListeners();
	}
	#endregion MonoBehaviour

	#region Public
	public void Inflate(string title, string[] paragraphs, Transform _panelRoot)
	{
		SetBackgroundColor();
		m_ContrastPanelRoot = _panelRoot;
		m_title = title;
		m_paragraphs = paragraphs;
	}

	public void AddListeners()
	{
		RemoveListeners();

		if (m_Button) m_Button.onClick.AddListener(OnButtonClick);
	}

	public void RemoveListeners()
	{
		if (m_Button) m_Button.onClick.RemoveListener(OnButtonClick);
	}

	private void SetBackgroundColor()
	{
		if (_icon) _icon.color = GlobalSettingsManager.Instance.AppColor;
	}
	#endregion Public

	#region Private
	private void OnButtonClick()
	{
		Instantiate(_contrastPanelPrefab, m_ContrastPanelRoot).Inflate(m_title, m_paragraphs);
	}
	#endregion Private
	#endregion Methods
}
