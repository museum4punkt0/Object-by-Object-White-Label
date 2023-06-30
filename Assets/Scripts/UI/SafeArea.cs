using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform m_panel;
    private Rect m_lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        m_panel = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != m_lastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect rect)
    {
        m_lastSafeArea = rect;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = rect.position;
        Vector2 anchorMax = rect.position + rect.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        m_panel.anchorMin = anchorMin;
        m_panel.anchorMax = anchorMax;

        Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, rect.x, rect.y, rect.width, rect.height, Screen.width, Screen.height);
    }
}