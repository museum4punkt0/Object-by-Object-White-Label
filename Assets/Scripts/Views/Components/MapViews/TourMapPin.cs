using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;

public class TourMapPin : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private int _referenceHeight = 80;
    [SerializeField] private float _referenceScale = 0.6f;
    [SerializeField] private float _highlightFactor = 1.3f;
    #endregion
    #region Private
    private Poi m_Poi;
    private Sprite m_DefaultSprite;
    private Transform m_Transform;
    private Vector3 m_DefaultScale;
    private Vector3 m_HighlightedScale;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Poi> TourMapPinClicked = new UnityEvent<Poi>();
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnMouseDown()
    {
        if (m_Poi != null) TourMapPinClicked?.Invoke(m_Poi);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitAndResize());
    }
    #endregion
    #region Public
    public void Inflate(Poi poi)
    {
        m_Poi = poi;
        name = m_Poi.title;
        PoiProgressionData poiProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).GetPoiProgression(m_Poi.pid);
        m_DefaultSprite = poiProgressionData.HasBeenVisited ? GlobalSettingsManager.Instance.TourPinSpriteCompleted : GlobalSettingsManager.Instance.TourPinSpriteNew;
        _spriteRenderer.sprite = m_DefaultSprite;
        _spriteRenderer.transform.localScale = _referenceScale * _referenceHeight/ m_DefaultSprite.texture.height * Vector3.one;
        m_Transform = transform;
        m_DefaultScale = m_Transform.localScale;
        m_HighlightedScale = m_DefaultScale * _highlightFactor;

    }

    public void Highlight(bool isHighlighted)
    {
        _spriteRenderer.sprite = isHighlighted ? GlobalSettingsManager.Instance.TourPinSpriteHighlighted : m_DefaultSprite;
        m_Transform.localScale = isHighlighted ? m_HighlightedScale : m_DefaultScale;
    }
    #endregion
    #region Private
    private IEnumerator WaitAndResize()
    {
        yield return null;
        Vector3 localScale = transform.localScale;
        transform.localScale = new Vector3(localScale.x, 0.1f, localScale.z);
    }
    #endregion
    #endregion
}
