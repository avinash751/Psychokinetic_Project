using System.Collections;
using UnityEngine;

public class DoorCloser : MonoBehaviour, IResettable
{
    [SerializeField] float delayUntilClose = 10f;
    [SerializeField] float timeToClose = 1.0f;
    [SerializeField] Transform closePosition;
    [Resettable] Transform t => transform;

    private void Start()
    {
        StartCoroutine(CloseDoor());
        ReferenceManager.Instance.Resetter.OnReset += () => StartCoroutine(CloseDoor());
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(delayUntilClose);
        float time = 0;
        Vector3 startPos = transform.position;
        while (time < timeToClose)
        {
            transform.position = Vector3.Lerp(startPos, closePosition.position, time / timeToClose);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = closePosition.position;
    }
}