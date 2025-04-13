using UnityEngine;

public class JacketTest : MonoBehaviour
{
    public Transform[] billowBones;  // Assign bones in the inspector
    public float amplitude = 0.5f;   // Amplitude of the sine wave motion
    public float frequency = 1.0f;   // Frequency of the sine wave motion

    void Update()
    {
        float wave = Mathf.Sin(Time.time * frequency) * amplitude;
        foreach (Transform bone in billowBones)
        {
            bone.localRotation = Quaternion.Euler(0, 0, wave);  // Rotate bones on the Z-axis
        }
    }
}


