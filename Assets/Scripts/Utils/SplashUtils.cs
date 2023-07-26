using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashUtils : MonoBehaviour
{
    #region Fields
    #region Private
    private static string m_splashBackSettingKey = "template.spk.loading.splashscreenback";
    private static string m_splashFrontSettingKey = "template.spk.loading.splashscreenfront";
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public static void DownloadSplashImages()
    {
        string splashBackSource = Wezit.Settings.Instance.GetSettingAsAssetSourceByTransformation(m_splashBackSettingKey);
        SpriteUtils.SaveTextureFromSource(splashBackSource, System.IO.Path.Combine(Application.dataPath, "Resources", "Images"), "splash_back");

        string splashSource = Wezit.Settings.Instance.GetSettingAsAssetSourceByTransformation(m_splashFrontSettingKey);
        SpriteUtils.SaveTextureFromSource(splashSource, System.IO.Path.Combine(Application.dataPath, "Resources", "Images"), "splash_front");
    }
    #endregion
    #region Private

    #endregion
    #endregion
}
