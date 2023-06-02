
using UnityEngine;

public class AnalyticsComponent
{
	private Wezit.AnalyticsService analyticsService;

  	private string screenEventAttrs = "";
	private string screenID;

	public string ScreenEventAttrs { get => screenEventAttrs; set => screenEventAttrs = value; }
	public string ScreenID { get => screenID; set => screenID = value; }

	public void SetScreenEventAttrs(string attrs)
	{
		ScreenEventAttrs = attrs;
	}

	public void AnalyticsBegin(MonoBehaviour mono)
	{
		if (!analyticsService)
		{
			analyticsService = Wezit.AnalyticsService.Instance;
		}
		mono.StartCoroutine(analyticsService.Send(new Wezit.Analytics(ScreenID, "begin", ScreenEventAttrs)));
	}

	public void AnalyticsEnd(MonoBehaviour mono)
	{
		if (!analyticsService)
		{
			analyticsService = Wezit.AnalyticsService.Instance;
		}
		mono.StartCoroutine(analyticsService.Send(new Wezit.Analytics(ScreenID, "end", ScreenEventAttrs)));
	}
}
