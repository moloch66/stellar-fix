using UnityEngine;
using System.Collections;

/// <summary>
/// Gerencia música ambiente e efeitos sonoros.
/// Fade suave entre faixas para manter o clima cozy.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Tracks")]
    [SerializeField] private AudioClip musicMainMenu;
    [SerializeField] private AudioClip musicWorkshop;

    [Header("SFX")]
    [SerializeField] private AudioClip sfxButtonClick;
    [SerializeField] private AudioClip sfxSparkle;
    [SerializeField] private AudioClip sfxRepairComplete;

    [Header("Settings")]
    [SerializeField][Range(0f,1f)] private float musicVolume = 0.5f;
    [SerializeField][Range(0f,1f)] private float sfxVolume   = 0.8f;
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Dois AudioSources: um para música, outro para SFX
        _musicSource = GetComponent<AudioSource>();
        _musicSource.loop   = true;
        _musicSource.volume = musicVolume;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop   = false;
        _sfxSource.volume = sfxVolume;
    }

    // ── Música ────────────────────────────────────────────────────────────────
    public void PlayMainMenuMusic()  => CrossfadeTo(musicMainMenu);
    public void PlayWorkshopMusic()  => CrossfadeTo(musicWorkshop);

    private void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;
        if (_musicSource.clip == clip && _musicSource.isPlaying) return;
        StartCoroutine(FadeCoroutine(clip));
    }

    private IEnumerator FadeCoroutine(AudioClip newClip)
    {
        // Fade out
        float t = 0f;
        float startVol = _musicSource.volume;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        _musicSource.clip = newClip;
        _musicSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }
        _musicSource.volume = musicVolume;
    }

    // ── SFX ───────────────────────────────────────────────────────────────────
    public void PlayClick()         => PlaySFX(sfxButtonClick);
    public void PlaySparkle()       => PlaySFX(sfxSparkle);
    public void PlayRepairComplete()=> PlaySFX(sfxRepairComplete);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ── Volume (para menu de configurações) ──────────────────────────────────
    public void SetMusicVolume(float v) { musicVolume = v; _musicSource.volume = v; }
    public void SetSFXVolume(float v)   { sfxVolume   = v; }
}
