﻿using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { private set; get; }
    #endregion

    public AudioData[] backgroundMusicList;
    public AudioData[] sfxList;

    [SerializeField] private AudioData currentMusic;

    [SerializeField] private AudioData prevMusic;

    #region Settings
    [HideInInspector] public bool isMusicMuted { get { return musicVolume <= 0; } }
    [HideInInspector] public bool isSfxMuted { get { return sfxVolume <= 0; } }

    [HideInInspector] public float musicVolume = 1f;
    [HideInInspector] public float sfxVolume = 1f;
    #endregion

    void Awake()
    {
        CreateInstance();

        SetUpAudioArray(backgroundMusicList);

        SetUpAudioArray(sfxList);

        ClearCurrentPrevMusic();
    }

    void Start()
    {
        PlayMusic("Music");
    }

    private void CreateInstance()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }

    private void SetUpAudioArray(AudioData[] _array)
    {
        for (int i = 0; i < _array.Length; i++)
        {
            GameObject child = new GameObject(_array[i].Name);
            child.transform.parent = transform;

            AudioSource audioSource = child.AddComponent<AudioSource>();

            _array[i].go = child;
            _array[i].audioSource = audioSource;

            _array[i].audioSource.clip = _array[i].audioClip;
            _array[i].audioSource.volume = _array[i].volume;
            _array[i].audioSource.pitch = _array[i].pitch;
            _array[i].audioSource.loop = _array[i].isLooping;
        }
    }

    private void ClearCurrentPrevMusic()
    {
        currentMusic = null;
        prevMusic = currentMusic;
    }

    public void PlayMusic(string _name)
    {
        Debug.Log("Playing Music");
        AudioData data = Array.Find(backgroundMusicList, bgm => bgm.Name == _name);
        if (data == null)
        {
            Debug.LogError("Didnt find music");
            return;
        }
        else
        {
            if (currentMusic != null)
                prevMusic = currentMusic;
            currentMusic = data;
            PlayNextMusicTrack();
        }
    }

    private void PlayNextMusicTrack()
    {
        currentMusic.audioSource.Play();
        if (!currentMusic.fade && prevMusic != null)
        {
            prevMusic.audioSource.Stop();
        }
        if (currentMusic.fade)
        {
            StartCoroutine(FadeIn(currentMusic));
            if (prevMusic.audioSource != null)
            {
                StartCoroutine(FadeOut(prevMusic));
            }
        }
    }

    public void PlaySFX(string _name)
    {
        AudioData data = Array.Find(sfxList, sfx => sfx.Name == _name);
        if (data == null)
        {
            Debug.LogError("Didnt find sound");
            return;
        }
        else
        {
            data.audioSource.Play();
        }
    }

    #region Settings
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        for (int i = 0; i < backgroundMusicList.Length; i++)
            backgroundMusicList[i].audioSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        for (int i = 0; i < sfxList.Length; i++)
            sfxList[i].audioSource.volume = volume;
    }
    #endregion

    #region Coroutines
    private IEnumerator FadeIn(AudioData _audioData)
    {
        _audioData.audioSource.volume = 0;
        float volume = _audioData.audioSource.volume;

        while (_audioData.audioSource.volume < _audioData.volume)
        {
            volume += _audioData.fadeInSpeed;
            _audioData.audioSource.volume = volume;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeOut(AudioData _audioData)
    {
        float volume = _audioData.audioSource.volume;

        while (_audioData.audioSource.volume > 0)
        {
            volume -= _audioData.fadeOutSpeed;
            _audioData.audioSource.volume = volume;
            yield return new WaitForSeconds(0.1f);
        }
        if (_audioData.audioSource.volume == 0)
        {
            _audioData.audioSource.Stop();
            _audioData.audioSource.volume = _audioData.volume;
        }
    }
    #endregion
}
