using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wezit;

public class GlobalSettingsManager : Singleton<GlobalSettingsManager>
{
    #region Fields
    private string m_AppColorSettingKey = "template.app.common.maincolor";
    private Color m_AppColor = Color.black;

    // Map Settings
    private string m_MapUserSettingKey = "template.spk.maps.common.user.location.image";
    private Sprite m_UserPinSprite;

    private string m_GlobalPinTextureSettingKey = "template.spk.maps.global.pin.background.image";
    private Texture m_GlobalPinTexture;

    private string m_TourPinSpriteNewSettingKey = "template.spk.maps.tour.pin.new.image";
    private Sprite m_TourPinSpriteNew;
    private string m_TourPinSpriteCompletedSettingKey = "template.spk.maps.tour.pin.visited.image";
    private Sprite m_TourPinSpriteCompleted;
    private string m_TourPinSpriteHighlightedSettingKey = "template.spk.maps.tour.pin.selected.image";
    private Sprite m_TourPinSpriteHighlighted;

    //Content Settings
    private string m_ContentAudioSpriteSettingKey = "template.spk.pois.AR.content.audio.image";
    private Sprite m_ContentAudioSprite;
    private string m_ContentImageSpriteSettingKey = "template.spk.pois.AR.content.image.image";
    private Sprite m_ContentImageSprite;
    private string m_ContentVideoSpriteSettingKey = "template.spk.pois.AR.content.video.image";
    private Sprite m_ContentVideoSprite;
    private string m_ContentAudioCompletedSpriteSettingKey = "template.spk.pois.AR.content.audio.completed.image";
    private Sprite m_ContentAudioCompletedSprite;
    private string m_ContentImageCompletedSpriteSettingKey = "template.spk.pois.AR.content.image.completed.image";
    private Sprite m_ContentImageCompletedSprite;
    private string m_ContentVideoCompletedSpriteSettingKey = "template.spk.pois.AR.content.video.completed.image";
    private Sprite m_ContentVideoCompletedSprite;
    private string m_PointsEarnedContentSettingKey = "template.spk.pois.content.points.earned.content";
    private int m_PointsEarnedContent;
    private string m_PointsEarnedSecretSettingKey = "template.spk.pois.content.points.earned.secret";
    private int m_PointsEarnedSecret;
    #endregion

    #region Properties
    public Color AppColor
    {
        get => m_AppColor;
    }

    public Color AppColorLight
    {
        get => new Color(AppColor.r, AppColor.g, AppColor.b, 0.588f);
    }

    // Map
    public Sprite UserPinSprite
    {
        get => m_UserPinSprite;
    }

    public Texture GlobalPinTexture
    {
        get => m_GlobalPinTexture;
    }

    public Sprite TourPinSpriteNew
    {
        get => m_TourPinSpriteNew;
    }

    public Sprite TourPinSpriteCompleted
    {
        get => m_TourPinSpriteCompleted;
    }

    public Sprite TourPinSpriteHighlighted
    {
        get => m_TourPinSpriteHighlighted;
    }

    // Content
    public Sprite ContentAudioSprite
    {
        get => m_ContentAudioSprite;
    }

    public Sprite ContentAudioCompletedSprite
    {
        get => m_ContentAudioCompletedSprite;
    }

    public Sprite ContentVideoSprite
    {
        get => m_ContentVideoSprite;
    }

    public Sprite ContentVideoCompletedSprite
    {
        get => m_ContentVideoCompletedSprite;
    }

    public Sprite ContentImageSprite
    {
        get => m_ContentImageSprite;
    }

    public Sprite ContentImageCompletedSprite
    {
        get => m_ContentImageCompletedSprite;
    }

    public int PointsEarnedContent
    {
        get => m_PointsEarnedContent;
    }

    public int PointsEarnedSecret
    {
        get => m_PointsEarnedSecret;
    }
    #endregion

    #region Methods
    #region Public
    public void Init()
    {
        m_AppColor = Settings.Instance.GetSettingAsColor(m_AppColorSettingKey);
        Camera.main.backgroundColor = m_AppColor;

        // Map settings
        string userPinSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_MapUserSettingKey);
        if (!string.IsNullOrEmpty(userPinSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(userPinSpriteSource, OnUserPinSpriteDownloaded, ""));
        }

        string globalPinTextureSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_GlobalPinTextureSettingKey);
        if (!string.IsNullOrEmpty(globalPinTextureSource))
        {
            StartCoroutine(SpriteUtils.GetTextureFromSource(globalPinTextureSource, OnGlobalPinTextureDownloaded, ""));
        }

        string tourPinSpriteNewSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_TourPinSpriteNewSettingKey);
        if (!string.IsNullOrEmpty(tourPinSpriteNewSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(tourPinSpriteNewSource, OnTourPinSpriteNewDownloaded, ""));
        }

        string tourPinSpriteCompletedSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_TourPinSpriteCompletedSettingKey);
        if (!string.IsNullOrEmpty(tourPinSpriteCompletedSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(tourPinSpriteCompletedSource, OnTourPinSpriteCompletedDownloaded, ""));
        }

        string tourPinSpriteHighlightedSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_TourPinSpriteHighlightedSettingKey);
        if (!string.IsNullOrEmpty(tourPinSpriteHighlightedSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(tourPinSpriteHighlightedSource, OnTourPinSpriteHighlightedDownloaded, ""));
        }

        // Content settings
        string contentAudioSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentAudioSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentAudioSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentAudioSpriteSource, OnContentAudioSpriteDownloaded, ""));
        }

        string contentImageSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentImageSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentImageSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentImageSpriteSource, OnContentImageSpriteDownloaded, ""));
        }

        string contentVideoSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentVideoSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentVideoSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentVideoSpriteSource, OnContentVideoSpriteDownloaded, ""));
        }

        string contentAudioCompletedSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentAudioCompletedSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentAudioCompletedSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentAudioCompletedSpriteSource, OnContentAudioCompletedSpriteDownloaded, ""));
        }

        string contentImageCompletedSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentImageCompletedSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentImageCompletedSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentImageCompletedSpriteSource, OnContentImageCompletedSpriteDownloaded, ""));
        }

        string contentVideoCompletedSpriteSource = Settings.Instance.GetSettingAsAssetSourceByTransformation(m_ContentVideoCompletedSpriteSettingKey);
        if (!string.IsNullOrEmpty(contentVideoCompletedSpriteSource))
        {
            StartCoroutine(SpriteUtils.GetSpriteFromSource(contentVideoCompletedSpriteSource, OnContentVideoCompletedSpriteDownloaded, ""));
        }

        m_PointsEarnedContent = (int)Settings.Instance.GetSettingAsFloat(m_PointsEarnedContentSettingKey);
        m_PointsEarnedSecret = (int)Settings.Instance.GetSettingAsFloat(m_PointsEarnedSecretSettingKey);
    }
    #endregion
    #region Private
    // Map icons
    private void OnUserPinSpriteDownloaded(Sprite sprite)
    {
        m_UserPinSprite = sprite;
    }

    private void OnGlobalPinTextureDownloaded(Texture texture)
    {
        m_GlobalPinTexture = texture;
    }

    private void OnTourPinSpriteNewDownloaded(Sprite sprite)
    {
        m_TourPinSpriteNew = sprite;
    }

    private void OnTourPinSpriteCompletedDownloaded(Sprite sprite)
    {
        m_TourPinSpriteCompleted = sprite;
    }

    private void OnTourPinSpriteHighlightedDownloaded(Sprite sprite)
    {
        m_TourPinSpriteHighlighted = sprite;
    }

    // Content icons
    private void OnContentAudioSpriteDownloaded(Sprite sprite)
    {
        m_ContentAudioSprite = sprite;
    }

    private void OnContentImageSpriteDownloaded(Sprite sprite)
    {
        m_ContentImageSprite = sprite;
    }

    private void OnContentVideoSpriteDownloaded(Sprite sprite)
    {
        m_ContentVideoSprite = sprite;
    }

    private void OnContentAudioCompletedSpriteDownloaded(Sprite sprite)
    {
        m_ContentAudioCompletedSprite = sprite;
    }

    private void OnContentImageCompletedSpriteDownloaded(Sprite sprite)
    {
        m_ContentImageCompletedSprite = sprite;
    }

    private void OnContentVideoCompletedSpriteDownloaded(Sprite sprite)
    {
        m_ContentVideoCompletedSprite = sprite;
    }
    #endregion
    #endregion
}
