using UnityEngine;

public class CircularScroller : MonoBehaviour
{
    [SerializeField] bool spin = false;
    [SerializeField] float radius = 100.0f;
    [SerializeField] float speed = 2.0f;

    RectTransform rectTransform;
    Vector2 initialPosition;
    float angle = 0.0f;

    void Update()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            Vector3 parentPos = transform.parent.localPosition;
            transform.parent.localPosition = Vector3.zero;
            initialPosition = transform.parent.position;
            transform.parent.localPosition = parentPos;
        }

        angle += speed * Time.unscaledDeltaTime;

        if (spin)
        {
            Vector3 rotation = new Vector3(0, 0, angle);
            rectTransform.localRotation = Quaternion.Euler(rotation);
        }

        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        rectTransform.position = initialPosition + new Vector2(x, y);
    }
}