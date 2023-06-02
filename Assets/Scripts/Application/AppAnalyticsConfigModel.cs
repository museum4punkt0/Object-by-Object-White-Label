using System;

[Serializable]
public class AppAnalyticsConfigModel
{
	public bool enabled;
	public string matomoUrl;
	public string websiteUrl;
	public string websiteId;

	public override string ToString()
	{
		return String.Format(
			"enabled: {0}\n" +
			"matomoUrl: {1}\n" +
			"websiteUrl: {2}\n" +
			"websiteId: {3}\n",
			enabled,
			matomoUrl,
			websiteUrl,
			websiteId
		);
	}
}
