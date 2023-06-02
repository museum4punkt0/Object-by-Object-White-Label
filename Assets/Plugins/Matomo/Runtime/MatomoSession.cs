//----------------------------------------
// MIT License
// Copyright(c) 2021 Jonas Boetel
//----------------------------------------
using System;
using System.Text;
using UnityEngine.Networking;

namespace Lumpn.Matomo
{
	public sealed class MatomoSession
	{
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly string baseUrl;
		private readonly Random random;

		private static string m_WebsiteUrl;

		public static MatomoSession Create(string matomoUrl, string websiteUrl, int websiteId, byte[] userHash)
		{
			m_WebsiteUrl = websiteUrl;

			var sb = new StringBuilder(matomoUrl);
			sb.Append("/matomo.php?apiv=1&rec=1&idsite=");
			sb.Append(websiteId);
			// sb.Append("&new_visit=1"); // useful to force NEW VISIT
			// sb.Append("&_id=");
			// HexUtils.AppendHex(sb, userHash);
			sb.Append("&cid=");
			sb.Append(Convert.ToBase64String(userHash, 4, 12));
			// sb.Append("&url=");
			// sb.Append(Uri.EscapeDataString(websiteUrl));
			// sb.Append(Uri.EscapeDataString("/"));

			var url = sb.ToString();
			var seed = CalculateSeed(userHash);
			return new MatomoSession(url, seed);
		}

		private MatomoSession(string baseUrl, int seed)
		{
			this.baseUrl = baseUrl;
			this.random = new Random(seed);
		}

		public UnityWebRequestAsyncOperation Record(string title, string page, float timespanSeconds)
		{
			var timespanMilliseconds = timespanSeconds * 1000;
			var url = BuildUrl(title, page, (int)timespanMilliseconds);
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, null, null);
			var op = request.SendWebRequest();
			return op;
		}

		private string BuildUrl(string title, string page, int timespanMilliseconds)
		{
			stringBuilder.Clear();

			stringBuilder.Append(baseUrl);

			if (!String.IsNullOrEmpty(page))
			{
				stringBuilder.Append("&url=");
				stringBuilder.Append(Uri.EscapeDataString(m_WebsiteUrl));
				stringBuilder.Append(Uri.EscapeDataString("/"));
				stringBuilder.Append(Uri.EscapeDataString(page));
			}

			stringBuilder.Append("&action_name=");
			stringBuilder.Append(Uri.EscapeDataString(title));
			stringBuilder.Append("&pf_net=");
			stringBuilder.Append(timespanMilliseconds);
			// stringBuilder.Append("&rand=");
			// stringBuilder.Append(random.Next());

			var url = stringBuilder.ToString();
			UnityEngine.Debug.Log("MatomoSession >>>> url >>>>> " + url);
			stringBuilder.Clear();

			return url;
		}

		private static int CalculateSeed(byte[] bytes)
		{
			unchecked
			{
				int result = 17;
				foreach (var value in bytes)
				{
					result = result * 23 + value;
				}
				return result;
			}
		}
	}
}
