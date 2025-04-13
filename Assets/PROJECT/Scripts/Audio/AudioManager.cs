using UnityEngine;
using MoreMountains.Tools;

public class AudioManager : MonoBehaviour
{
    public SoundLibrary soundLibrary;
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public AudioSource PlayAudio(string key)
    {
        if (soundLibrary.soundOptionsDictionary.TryGetValue(key, out var soundOptions))
        {
            return InitializeAudioThenPlay(soundOptions);
        }
        return null;
    }

    public AudioSource PlayAudio(SoundOptions soundOptions)
    {
        AudioSource audioSource = InitializeAudioThenPlay(soundOptions);
        return audioSource;
    }

    public void ButtonPlay(SoundOptions soundOptions)
    {
        InitializeAudioThenPlay(soundOptions);
    }

    private AudioSource InitializeAudioThenPlay(SoundOptions soundOptions)
    {
        MMSoundManagerPlayOptions playOptions = soundOptions.soundPlayOptions;

        RandomizeValueBetweenMinAndMax(ref playOptions.Volume, soundOptions.minVolume, soundOptions.maxVolume);
        RandomizeValueBetweenMinAndMax(ref playOptions.Pitch, soundOptions.minPitch, soundOptions.maxPitch);

        AudioClip clipToPlay = null;
        if (soundOptions.randomAudioClips != null && soundOptions.randomAudioClips.Length > 0)
        {
            int index = Random.Range(0, soundOptions.randomAudioClips.Length);
            clipToPlay = soundOptions.randomAudioClips[index];
        }
        if (clipToPlay == null)
        {
            clipToPlay = soundOptions.audioClip;
        }

        AudioSource audioSource = MMSoundManagerSoundPlayEvent.Trigger(clipToPlay, playOptions);
        return audioSource;
    }

    public void StopAudio(string key)
    {
        if (soundLibrary.soundOptionsDictionary.TryGetValue(key, out var soundOptions))
        {
            MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Free, soundOptions.soundPlayOptions.ID);
        }
    }

    public void StopAudio(SoundOptions soundOptions)
    {
        MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Free, soundOptions.soundPlayOptions.ID);
    }

    public void FadeOut(SoundOptions sound)
    {
        var soundPlayOptions = sound.soundPlayOptions;
        var ID = soundPlayOptions.ID;
        var fadeDuration = soundPlayOptions.FadeDuration;
        var fadeTween = soundPlayOptions.FadeTween;
        MMSoundManagerSoundFadeEvent.Trigger(MMSoundManagerSoundFadeEvent.Modes.StopFade, ID, fadeDuration, 0, fadeTween);
    }

    public void RandomizeValueBetweenMinAndMax(ref float valueToAlter, float min, float max)
    {
        if (min == 0 && max == 0) { return; }
        if (min == 1 && max == 1) { return; }
        valueToAlter = Random.Range(min, max);
    }
}