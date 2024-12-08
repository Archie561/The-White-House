using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _backgroundMusic;

    private const float FADE_DURATION = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusic(string fileName)
    {
        Addressables.LoadAssetAsync<AudioClip>("Audio/Music/" + fileName).Completed += OnMusicClipLoaded;
    }
    private void OnMusicClipLoaded(AsyncOperationHandle<AudioClip> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            StopAllCoroutines();
            StartCoroutine(PlayAudio(handle.Result));
        }
        else
        {
            Debug.LogError("Audio clip failed to load.");
        }
    }

    public void PlaySFX(string fileName)
    {
        Addressables.LoadAssetAsync<AudioClip>("Audio/SFX/" + fileName).Completed += OnSFXClipLoaded;
    }
    private void OnSFXClipLoaded(AsyncOperationHandle<AudioClip> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _sfxSource.PlayOneShot(handle.Result);
        }
        else
        {
            Debug.LogError("Audio clip failed to load.");
        }
    }

    public void StopPlaying()
    {
        if (_musicSource.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAudio());
        }
    }

    public void SetMusicVolume(float volume) => _musicSource.volume = volume;

    public void SetSFXVolume(float volume) => _sfxSource.volume = volume;

    private IEnumerator PlayAudio(AudioClip clip)
    {
        if (_musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeAudio());
        }

        _musicSource.clip = clip;
        _musicSource.Play();

        yield return new WaitForSeconds(clip.length);
        StartCoroutine(PlayAudio(_backgroundMusic));
    }

    private IEnumerator FadeAudio()
    {
        float initialVolume = _musicSource.volume;
        float timeElapsed = 0f;
        while (timeElapsed < FADE_DURATION)
        {
            _musicSource.volume = Mathf.Lerp(initialVolume, 0, timeElapsed / FADE_DURATION);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _musicSource.Stop();
        _musicSource.volume = initialVolume;
    }
}