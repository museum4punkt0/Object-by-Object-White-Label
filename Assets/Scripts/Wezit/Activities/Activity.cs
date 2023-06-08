using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;
using System.Threading.Tasks;

namespace Wezit
{
    public class Activity : MonoBehaviour
    {
        #region Fields
        #region SerializeFields
        [SerializeField] private Transform _beginRoot = null;
        [SerializeField] private Transform _endRoot = null;
        #endregion
        #region Private
        private bool m_HasBegin = false;
        private bool m_HasEnd = false;
        private float m_ChronoValue = 0;

        private ActivityBegin m_Begin = null;
        private ActivityEnd m_End = null;

        #endregion
        #region Internal
        internal string m_Type = ActivityType.DEFAULT;
        internal JSONNode m_ActivityNode;
        internal Language m_Language;
        internal bool m_HasChrono = false;
        internal string m_BeginPrefabName = "Prefabs/ActivityPrefabs/Begin";
        internal string m_EndPrefabName = "Prefabs/ActivityPrefabs/End";
        #endregion
        #endregion

        #region Properties
        public string Type { get { return m_Type; } }
        public UnityEvent ActivityOver = new();
        #endregion

        #region Methods
        #region Monobehaviour
        #endregion
        #region Public
        public virtual void Inflate(JSONNode activityNode, Language language)
        {
            m_ActivityNode = activityNode;
            m_Language = language;
            m_HasBegin = GetKeyNodeForLanguage(language, "template.activity.begin.activation");
            m_HasEnd = GetKeyNodeForLanguage(language, "template.activity.end.activation");
            m_Type = GetKeyNodeForLanguage(language, "template.app.common.type");
            m_HasChrono = GetKeyNodeForLanguage(language, "template.activity.chrono.activation");
            if(m_HasChrono) m_ChronoValue = GetKeyNodeForLanguage(language, "template.activity.chrono.value");

            InitContent();
        }
        #endregion
        #region Internal
        #endregion
        #region Private
        internal virtual void InitContent()
        {
            if (m_HasBegin && _beginRoot)
            {
                m_Begin = Instantiate(Resources.Load<ActivityBegin>(m_BeginPrefabName), _beginRoot);
                m_Begin.Inflate(m_ActivityNode, m_Language);
            }
            if (m_HasEnd && _endRoot)
            {
                m_End = Instantiate(Resources.Load<ActivityEnd>(m_EndPrefabName), _endRoot);
            }
        }

        internal async Task LoadImage(Language language, string key, RawImage imageComponent)
        {
            string imageName = StringUtils.CleanFromWezit(GetKeyNodeForLanguage(language, key));
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("wzasset://", "");
                WezitAssets.Asset asset = AssetsLoader.GetAssetById(imageName);
                await StartCoroutine(Utils.ImageUtils.SetImage(imageComponent,
                                                         asset.GetAssetSourceByTransformation(WezitSourceTransformation.original),
                                                         asset.GetAssetMimeTypeByTransformation(WezitSourceTransformation.original),
                                                         true));
            }
        }

        internal async Task LoadImage(string imageName, RawImage imageComponent)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("wzasset://", "");
                WezitAssets.Asset asset = AssetsLoader.GetAssetById(imageName);
                await StartCoroutine(Utils.ImageUtils.SetImage(imageComponent,
                                                         asset.GetAssetSourceByTransformation(WezitSourceTransformation.original),
                                                         asset.GetAssetMimeTypeByTransformation(WezitSourceTransformation.original),
                                                         true));
            }
        }

        internal async Task GetTextureForKey(Language language, string key, System.Action<Texture2D> result)
        {
            string imageName = StringUtils.CleanFromWezit(GetKeyNodeForLanguage(language, key));
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("wzasset://", "");
                WezitAssets.Asset asset = AssetsLoader.GetAssetById(imageName);
                await StartCoroutine(SpriteUtils.GetTextureFromSource(
                                                         asset.GetAssetSourceByTransformation(WezitSourceTransformation.original),
                                                         result,
                                                         key));
            }
        }

        internal JSONNode GetKeyNodeForLanguage(Language language, string key)
        {
            JSONNode keyNode = m_ActivityNode[language.ToString()][key];
            if(keyNode == null)
            {
                keyNode = m_ActivityNode["default"][key];
            }
            return keyNode;
        }
        #endregion
        #endregion
    }
}
