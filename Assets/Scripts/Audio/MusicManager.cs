using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] public MusicLibrary musicLibrary;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private RhythmChecker rhythmChecker;

    public string currentMusicPlaying;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignDependencies();
    }
    private void AssignDependencies()
    {
        if (musicSource == null)
        {
            Music musicObject = FindObjectOfType<Music>();
            if (musicObject != null)
            {
                musicSource = musicObject.GetComponent<AudioSource>();
                Debug.Log($"MusicSource reassigned successfully from Music script: {musicObject.name}");
            }
            else
            {
                Debug.LogError("No Music object found in the scene. MusicManager requires one.");
            }
        }

        if (musicLibrary == null)
        {
            musicLibrary = FindObjectOfType<MusicLibrary>();
            if (musicLibrary != null)
            {
                Debug.Log("MusicLibrary reassigned successfully.");
            }
            else
            {
                Debug.LogError("No MusicLibrary found in the scene. MusicManager requires one.");
            }
        }

        if (rhythmChecker == null)
        {
            rhythmChecker = FindObjectOfType<RhythmChecker>();
            if (rhythmChecker != null)
            {
                Debug.Log("RhythmChecker reassigned successfully.");
            }
            else
            {
                Debug.LogError("No RhythmChecker found in the scene. MusicManager requires one.");
            }
        }
    }
    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        Debug.Log($"Playing music: {trackName}");

        MusicTrack track = musicLibrary.GetTrack(trackName);
        rhythmChecker.UpdateData(track.bpm, track.offset);
        StartCoroutine(AnimateMusicCrossFade(musicLibrary.GetAudioClip(trackName), fadeDuration));
        currentMusicPlaying = trackName;
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1.0f / fadeDuration;
            musicSource.volume = Mathf.Lerp(1.0f, 0.0f, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1.0f / fadeDuration;
            musicSource.volume = Mathf.Lerp(0.0f, 1.0f, percent);
            yield return null;
        }
        yield return null;
    }

    public void UpdateConductor()
    {
        
    }
}
