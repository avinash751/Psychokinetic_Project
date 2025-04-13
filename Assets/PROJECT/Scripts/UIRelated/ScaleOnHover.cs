using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,ISelectHandler, IDeselectHandler
{
    [SerializeField] float scaleMultiplier = 1.1f;
    Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = originalScale;
    }
}