// TouchLookArea.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLookArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector2 touchOrigin;

    public void OnPointerDown(PointerEventData eventData)
    {
        touchOrigin = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InputManager.Instance == null) return;
        Vector2 delta = eventData.position - touchOrigin;
        InputManager.Instance.SetLookInput(delta);
        touchOrigin = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.SetLookInput(Vector2.zero);
    }
}