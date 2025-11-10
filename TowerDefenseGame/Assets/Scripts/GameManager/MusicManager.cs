using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private AudioSource musicSource;

    //Variables
    [SerializeField] public List<AudioDictionary> musicDictionary;
    [SerializeField] private Dictionary<string, AudioClip> adaptiveMusic;

    private bool isFading;

    //FOR SFX
    //[SerializeField] public List<AudioDictionary> sfxDict;
    //[SerializeField] private Dictionary<string, AudioClip> gameSFX;

    public float fadeTime;
    [Range(0f, 1f)] public float musicVolume;

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        adaptiveMusic = new Dictionary<string, AudioClip>();
        BuildMusicDictionary();
    }

    private void BuildMusicDictionary()
    {
        foreach(var entry in musicDictionary)
        {
            if (!adaptiveMusic.ContainsKey(entry.Key))
            {
                adaptiveMusic.Add(entry.Key, entry.Value);
            }
        }
    }

    public void PlayMusic(string clipName, bool willFade)
    {
        AudioClip clip = adaptiveMusic[clipName];
        musicSource.clip = clip;

        if (willFade)
        {            

            StartCoroutine(FadeIn());


        }
        else
        {
            musicSource.volume = musicVolume;
            musicSource.Play();

        }

    }

    public IEnumerator FadeIn()
    {
        if(isFading)
        {
            while (isFading)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        musicSource.volume = 0;
        musicSource.Play();

        while(musicSource.volume < musicVolume)
        {
            musicSource.volume += 0.1f;  
            yield return new WaitForSeconds(fadeTime);
        }
    }

    public void StopMusic(bool willFade)
    {
        if (willFade)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            musicSource.Stop();
        }
    }

    public IEnumerator FadeOut()
    {
        isFading = true;
        while(musicSource.volume > 0)
        {
            musicSource.volume = musicSource.volume - 0.1f;
            yield return new WaitForSeconds(fadeTime);
        }

        musicSource.Stop();
        isFading = false;

    }

}

[System.Serializable]
public class AudioDictionary
{
    public string Key;
    public AudioClip Value;
}
