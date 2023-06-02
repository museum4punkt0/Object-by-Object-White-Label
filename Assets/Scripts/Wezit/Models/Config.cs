using System;

namespace Wezit
{
	[Serializable]
	public class ConfigModel
	{
		public string manifestUrl;
		public string version;
		public bool online;
		public bool loadImages;
		public bool downloadImagesOnStartup;
		public string downloadTransformation;

		public override string ToString() 
		{
			return string.Format(
				"manifestUrl: {0}\n" +
                "version: {1}\n" +
				"online: {2}\n" +
				"loadImages: {3}\n" +
				"downloadImagesOnStartup: {4}\n" +
				"downloadTransformation: {5}",
				manifestUrl,
				version,
				online,
				loadImages,
				downloadImagesOnStartup,
				downloadTransformation
			);
		}
	}
}
