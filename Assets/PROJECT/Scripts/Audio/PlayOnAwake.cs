using UnityEngine;
using VInspector;

public class PlayOnAwake : MonoBehaviour
{
    [SerializeField] string audioKey = string.Empty;
    [SerializeField] protected bool useSoundOptionsSO;

    public AudioSource source { get; protected set; }

    [ShowIf("useSoundOptionsSO")]
    [SerializeField] SoundOptions soundSO;
    [EndIf]

    void Start()
    {
        PlayAudio();
    }

    protected virtual void PlayAudio()
    {
        if (!useSoundOptionsSO)
        { 
            source = AudioManager.Instance?.PlayAudio(audioKey); 
        }
        else
        { 
            source = AudioManager.Instance?.PlayAudio(soundSO); 
        }
    }
}