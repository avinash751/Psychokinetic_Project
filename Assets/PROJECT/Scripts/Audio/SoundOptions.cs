using UnityEngine;
using MoreMountains.Tools;


[CreateAssetMenu(fileName = "Sound Options")]
public class SoundOptions : ScriptableObject
{
    public AudioClip audioClip;
    [Range(0,1)]public float minVolume;
    [Range(0,1)]public float maxVolume;
    [Range(0,3)]public float minPitch;
    [Range(0,3)]public float maxPitch;
    public AudioClip[] randomAudioClips;
    public MMSoundManagerPlayOptions soundPlayOptions;

    private void OnValidate()
    {
        soundPlayOptions.ID = name.GetHashCode();
    }
}