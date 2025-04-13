using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFXFuncs : MonoBehaviour
{
    [SerializeField] private float pulseScale = 0.7f;
    [SerializeField] private Vector3 startSize;
    [SerializeField] private float _returnSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        startSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * _returnSpeed);
    }
    public void Pulse()
    {
        transform.localScale = startSize * pulseScale;
    }
}
