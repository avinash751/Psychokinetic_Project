using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
public class AudioPulse : MonoBehaviour
{
    public AudioSource audioSource;
    public float sensitivity = 2.0f; 
    public float baseScale = 1.0f; 
    public float lerpSpeed = 1.0f; 

    public float rotationSpeed = 30.0f;
    public float hoverAmplitude = 0.5f; 
    public float hoverFrequency = 1.0f; 

    private float[] spectrumData = new float[64];
    private Vector3 initialScale;
    private Vector3 initialPosition;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (audioSource == null) { audioSource = MusicTrack.Instance.source; }
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        float average = 0.0f;
        foreach (float value in spectrumData)
        {
            average += value;
        }
        average /= spectrumData.Length;

        float scaleFactor = baseScale + average * sensitivity;

        Vector3 targetScale = initialScale * scaleFactor;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + hoverOffset, initialPosition.z);
    }
}
