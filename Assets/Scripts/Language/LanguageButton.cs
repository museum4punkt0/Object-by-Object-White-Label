using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
	#region Fields
	public UnityEvent<Language> LanguageButtonClicked = new();

	internal Button m_Button;
	[SerializeField] private TextMeshProUGUI _labelTitle;
	[SerializeField] private Image _background;
	private Language m_Language;
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	internal void Awake()
	{
		m_Button = GetComponent<Button>();

		SetLabelTitle("");

		AddListeners();
	}
	#endregion MonoBehaviour

	#region Public
	public void Inflate(Language language, string label, Color color)
    {
		m_Language = language;
		SetLabelTitle(label);
		SetBackgroundColor(color);
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

	public void SetLabelTitle(string textContent)
	{
		if (_labelTitle) _labelTitle.text = textContent;
		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_labelTitle.transform.parent.gameObject));
	}

	private void SetBackgroundColor(Color bgColor)
    {
		if (_background) _background.color = bgColor;
    }
	#endregion Public

	#region Private
	private void OnButtonClick()
	{
		LanguageButtonClicked?.Invoke(m_Language);
	}
	#endregion Private
	#endregion Methods
}
