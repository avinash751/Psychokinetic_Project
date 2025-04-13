using System.Collections.Generic;
using UnityEngine;

public class MusicTrack : MonoBehaviour
{
    [SerializeField] List<SoundOptions> musicTracks;

    public static MusicTrack Instance;
    public AudioSource source { get; protected set; }
    AudioSource previousSource;

    List<SoundOptions> usableTracks = new();
    SoundOptions lastTrack;
    SoundOptions currentTrack;

    bool isFading = false;
    float fadeDuration;
    float fadeTimer;
    float initialVolume;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayAudio();
    }

    protected void PlayAudio()
    {
        if (usableTracks.Count == 0) { usableTracks = new List<SoundOptions>(musicTracks); }

        lastTrack = currentTrack;
        do
        {
            currentTrack = usableTracks[Random.Range(0, usableTracks.Count)];
        }
        while (currentTrack == lastTrack && usableTracks.Count > 1);
        usableTracks.Remove(currentTrack);
        previousSource = source;
        source = AudioManager.Instance.PlayAudio(currentTrack);
    }

    void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (fadeTimer < fadeDuration)
            {
                float newVolume = Mathf.Lerp(initialVolume, 0f, fadeTimer / fadeDuration);
                previousSource.volume = newVolume;
            }
            else
            {
                EndFade();
            }
        }
    }

    private void EndFade()
    {
        previousSource.volume = 0f;
        AudioManager.Instance.StopAudio(lastTrack);
        fadeTimer = 0f;
        isFading = false;
    }

    public void CrossFade()
    {
        if (isFading)
        {
            EndFade();
        }

        PlayAudio();

        if (previousSource != null)
        {
            fadeDuration = lastTrack.soundPlayOptions.FadeDuration;
            fadeTimer = 0f;
            initialVolume = previousSource.volume;
            isFading = true;
        }
    }

    private void OnEnable()
    {
        LevelUIManager.OnLevelChange += CrossFade;
    }

    private void OnDisable()
    {
        LevelUIManager.OnLevelChange -= CrossFade;
    }
}