using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


#region Data Containers

/// <summary>
/// One entry in your Music list. Fill out in the Inspector.
/// </summary>
[Serializable]
public class MusicTrack
{
    [Tooltip("Unique key used in code: AudioManager.Instance.PlayMusic(\"MainTheme\")")]
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    public bool loop = true;

    [HideInInspector] public AudioSource source;
}

/// <summary>
/// One entry in your Sound Effects list. Fill out in the Inspector.
/// </summary>
[Serializable]
public class SoundEffect
{
    [Tooltip("Unique key used in code: AudioManager.Instance.PlaySFX(\"Jump\")")]
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    /// <summary>Randomise pitch slightly on every play for variety.</summary>
    public bool randomisePitch = false;

    [Range(0f, 0.5f)]
    [Tooltip("Max offset applied to pitch when randomisePitch is true.")]
    public float pitchVariance = 0.1f;

    [HideInInspector] public AudioSource source;
}

#endregion

// -------------------------------------------------------------

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject and attach AudioManager if none exists
                GameObject audioManagerObject = new GameObject("AudioManager");
                _instance = audioManagerObject.AddComponent<AudioManager>();
                DontDestroyOnLoad(audioManagerObject);
            }
            return _instance;
        }
    }

    // -- Inspector Fields --------------------------------------
    [Header("Mixer (optional)")]
    [Tooltip("Assign your AudioMixer if you want snapshot / mixer-group support.")]
    public AudioMixer mixer;

    [Tooltip("Mixer group for music. Leave empty to skip.")]
    public AudioMixerGroup musicMixerGroup;

    [Tooltip("Mixer group for SFX. Leave empty to skip.")]
    public AudioMixerGroup sfxMixerGroup;

    [Header("Music Tracks")]
    public MusicTrack[] music;

    [Header("Sound Effects")]
    public SoundEffect[] sounds;

    [Header("Fade Settings")]
    [Range(0.1f, 5f)] public float defaultFadeDuration = 1f;

    // -- Private State -----------------------------------------
    private MusicTrack _currentTrack;
    private Coroutine _fadeCoroutine;

    // ---------------------------------------------------------
    #region Unity Lifecycle

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitialiseMusicTracks();
        InitialiseSoundEffects();
    }

    #endregion

    // ---------------------------------------------------------
    #region Initialisation

    private void InitialiseMusicTracks()
    {
        foreach (MusicTrack track in music)
        {
            if (track.clip == null)
            {
                Debug.LogWarning($"[AudioManager] Music track '{track.name}' has no AudioClip assigned.");
                continue;
            }

            track.source = gameObject.AddComponent<AudioSource>();
            track.source.clip = track.clip;
            track.source.volume = track.volume;
            track.source.pitch = track.pitch;
            track.source.loop = track.loop;
            track.source.playOnAwake = false;

            if (musicMixerGroup != null)
                track.source.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    private void InitialiseSoundEffects()
    {
        foreach (SoundEffect sfx in sounds)
        {
            if (sfx.clip == null)
            {
                Debug.LogWarning($"[AudioManager] SFX '{sfx.name}' has no AudioClip assigned.");
                continue;
            }

            sfx.source = gameObject.AddComponent<AudioSource>();
            sfx.source.clip = sfx.clip;
            sfx.source.volume = sfx.volume;
            sfx.source.pitch = sfx.pitch;
            sfx.source.loop = false;
            sfx.source.playOnAwake = false;

            if (sfxMixerGroup != null)
                sfx.source.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    #endregion


    #region Music API

    /// <summary>Play a music track immediately (no fade).</summary>
    public void PlayMusic(string trackName)
    {
        MusicTrack track = FindMusic(trackName);
        if (track == null) return;

        if (_currentTrack != null && _currentTrack != track)
            _currentTrack.source.Stop();

        _currentTrack = track;
        track.source.volume = track.volume;
        track.source.Play();
    }

    /// <summary>Cross-fade to a new music track.</summary>
    public void PlayMusicWithFade(string trackName, float fadeDuration = -1f)
    {
        MusicTrack track = FindMusic(trackName);
        if (track == null) return;

        float duration = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(CrossFade(_currentTrack, track, duration));
        _currentTrack = track;
    }

    /// <summary>Stop the currently playing music (optional fade-out).</summary>
    public void StopMusic(bool fade = false, float fadeDuration = -1f)
    {
        if (_currentTrack == null) return;

        if (fade)
        {
            float duration = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeOut(_currentTrack, duration));
        }
        else
        {
            _currentTrack.source.Stop();
        }
    }

    /// <summary>Pause the current music track.</summary>
    public void PauseMusic() => _currentTrack?.source.Pause();

    /// <summary>Resume the current music track.</summary>
    public void ResumeMusic() => _currentTrack?.source.UnPause();

    /// <summary>Set music volume at runtime (0-1).</summary>
    public void SetMusicVolume(float volume)
    {
        if (_currentTrack == null) return;
        _currentTrack.source.volume = Mathf.Clamp01(volume);
    }

    /// <summary>Is this track currently playing?</summary>
    public bool IsMusicPlaying(string trackName)
    {
        MusicTrack track = FindMusic(trackName);
        return track != null && track.source != null && track.source.isPlaying;
    }

    #endregion

    #region SFX API

    /// <summary>Play a sound effect.</summary>
    public AudioSource PlaySFX(string soundName)
    {
        SoundEffect sfx = FindSFX(soundName);
        if (sfx == null) return null;

        sfx.source.pitch = sfx.randomisePitch
            ? sfx.pitch + UnityEngine.Random.Range(-sfx.pitchVariance, sfx.pitchVariance)
            : sfx.pitch;

        sfx.source.Play();
        return sfx.source;
    }

    /// <summary>Play a sound effect at a world-space position (uses PlayClipAtPoint).</summary>
    public void PlaySFXAtPosition(string soundName, Vector3 position)
    {
        SoundEffect sfx = FindSFX(soundName);
        if (sfx == null) return;

        float pitch = sfx.randomisePitch
            ? sfx.pitch + UnityEngine.Random.Range(-sfx.pitchVariance, sfx.pitchVariance)
            : sfx.pitch;

        // PlayClipAtPoint doesn't support pitch; spawn a temporary source instead.
        GameObject tempGO = new GameObject($"SFX_{soundName}");
        tempGO.transform.position = position;
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = sfx.clip;
        tempSource.volume = sfx.volume;
        tempSource.pitch = pitch;

        if (sfxMixerGroup != null)
            tempSource.outputAudioMixerGroup = sfxMixerGroup;

        tempSource.Play();
        Destroy(tempGO, sfx.clip.length / Mathf.Abs(pitch) + 0.1f);
    }

    /// <summary>Stop a looping SFX.</summary>
    public void StopSFX(string soundName)
    {
        SoundEffect sfx = FindSFX(soundName);
        sfx?.source.Stop();
    }

    /// <summary>Set volume for a specific SFX (0-1).</summary>
    public void SetSFXVolume(string soundName, float volume)
    {
        SoundEffect sfx = FindSFX(soundName);
        if (sfx != null) sfx.source.volume = Mathf.Clamp01(volume);
    }

    /// <summary>Is this SFX currently playing?</summary>
    public bool IsSFXPlaying(string soundName)
    {
        SoundEffect sfx = FindSFX(soundName);
        return sfx != null && sfx.source != null && sfx.source.isPlaying;
    }

    #endregion

    // ---------------------------------------------------------
    #region Global Volume

    /// <summary>Mute / unmute all audio.</summary>
    public void SetMute(bool mute) => AudioListener.pause = mute;

    /// <summary>Master volume via AudioListener (0-1).</summary>
    public void SetMasterVolume(float volume) =>
        AudioListener.volume = Mathf.Clamp01(volume);

    /// <summary>Control a named exposed mixer parameter (e.g. "MusicVolume") in dB.</summary>
    public void SetMixerVolume(string parameterName, float normalised)
    {
        if (mixer == null) return;
        // Convert 0-1 to a useful dB range (-80 to 0)
        float dB = normalised > 0.0001f ? Mathf.Log10(normalised) * 20f : -80f;
        mixer.SetFloat(parameterName, dB);
    }

    #endregion

    // ---------------------------------------------------------
    #region Coroutines

    private IEnumerator CrossFade(MusicTrack outTrack, MusicTrack inTrack, float duration)
    {
        // Start the incoming track silently
        inTrack.source.volume = 0f;
        inTrack.source.Play();

        float elapsed = 0f;
        float outStart = outTrack != null ? outTrack.source.volume : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            if (outTrack != null)
                outTrack.source.volume = Mathf.Lerp(outStart, 0f, t);

            inTrack.source.volume = Mathf.Lerp(0f, inTrack.volume, t);

            yield return null;
        }

        outTrack?.source.Stop();
        inTrack.source.volume = inTrack.volume;
    }

    private IEnumerator FadeOut(MusicTrack track, float duration)
    {
        float startVolume = track.source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            track.source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        track.source.Stop();
        track.source.volume = startVolume; // restore for next play
    }

    #endregion

    // ---------------------------------------------------------
    #region Helpers

    private SoundEffect FindSFX(string soundName)
    {
        if (sounds == null)
        {
            Debug.LogError("[AudioManager] Sounds array is null.");
            return null;
        }

        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogError("[AudioManager] Sound name is null or empty.");
            return null;
        }

        SoundEffect sfx = Array.Find(sounds, s => s.name == soundName);
        if (sfx == null)
            Debug.LogWarning($"[AudioManager] SFX '{soundName}' not found. Check the name spelling.");

        return sfx;
    }

    private MusicTrack FindMusic(string trackName)
    {
        if (music == null)
        {
            Debug.LogError("[AudioManager] Music array is null.");
            return null;
        }

        if (string.IsNullOrEmpty(trackName))
        {
            Debug.LogError("[AudioManager] Track name is null or empty.");
            return null;
        }

        MusicTrack track = Array.Find(music, m => m.name == trackName);
        if (track == null)
            Debug.LogWarning($"[AudioManager] Music track '{trackName}' not found. Check the name spelling.");

        return track;
    }

    #endregion
}

// -------------------------------------------------------------
//  QUICK REFERENCE
// -------------------------------------------------------------
//
//  MUSIC
//  -----
//  AudioManager.Instance.PlayMusic("MainTheme");
//  AudioManager.Instance.PlayMusicWithFade("BossTheme", 2f);
//  AudioManager.Instance.StopMusic(fade: true);
//  AudioManager.Instance.PauseMusic();
//  AudioManager.Instance.ResumeMusic();
//  AudioManager.Instance.SetMusicVolume(0.5f);
//  AudioManager.Instance.IsMusicPlaying("MainTheme");
//
//  SFX
//  ---
//  AudioManager.Instance.PlaySFX("Jump");
//  AudioManager.Instance.PlaySFXAtPosition("Explosion", transform.position);
//  AudioManager.Instance.StopSFX("Footsteps");
//  AudioManager.Instance.SetSFXVolume("Jump", 0.8f);
//  AudioManager.Instance.IsSFXPlaying("Footsteps");
//
//  GLOBAL
//  ------
//  AudioManager.Instance.SetMasterVolume(0.75f);
//  AudioManager.Instance.SetMute(true);
//  AudioManager.Instance.SetMixerVolume("MusicVolume", 0.5f); // needs AudioMixer
//
// -------------------------------------------------------------