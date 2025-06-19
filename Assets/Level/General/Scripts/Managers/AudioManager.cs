using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private Dictionary<SFXType, AudioClip> _sfxClips;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllSFX();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllSFX()
    {
        var allSFX = Resources.LoadAll<AudioClip>("Audio/SFX");
        _sfxClips = allSFX.ToDictionary(
            clip => (SFXType)Enum.Parse(typeof(SFXType), clip.name),
            clip => clip
        );
    }

    public void PlaySFX(SFXType sfxType)
    {
        if (_sfxClips.TryGetValue(sfxType, out var clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX {sfxType} not found!");
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (_musicSource.clip != null)
        {
            _musicSource.Stop();
        }
        _musicSource.clip = musicClip;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        if (_musicSource.clip != null)
        {
            _musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume) => _musicSource.volume = volume;

    public void SetSFXVolume(float volume) => _sfxSource.volume = volume;
}