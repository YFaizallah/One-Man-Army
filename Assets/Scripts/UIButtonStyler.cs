using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonStyler : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale States")]
    public float idleScale = 0.4f;
    public float hoverScale = 0.48f;
    public float pressedScale = 0.36f;

    [Header("Animation")]
    public float speed = 14f;

    private RectTransform rt;
    private Vector3 target;

    void Awake()
    {
        rt = (RectTransform)transform;

        // Start and stay idle
        rt.localScale = Vector3.one * idleScale;
        target = Vector3.one * idleScale;
    }

    void Update()
    {
        // Animate even when paused
        rt.localScale = Vector3.Lerp(
            rt.localScale,
            target,
            Time.unscaledDeltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        target = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target = Vector3.one * idleScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        target = Vector3.one * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        target = Vector3.one * hoverScale;
    }
}
