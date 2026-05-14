using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public enum AreaType
    {
        Forest,
        Mountain,
        Beach,
        Spawn
    }

    [Header("Music")]
    public AudioSource musicSource;

    [Header("Random Ambience")]
    public AudioSource ambienceSource;

    [Header("Music Clips")]
    public AudioClip forestMusic;
    public AudioClip mountainMusic;
    public AudioClip beachMusic;
    public AudioClip spawnMusic;

    [Header("Ambience Lists")]
    public List<AudioClip> forestAmbience = new List<AudioClip>();
    public List<AudioClip> mountainAmbience = new List<AudioClip>();
    public List<AudioClip> beachAmbience = new List<AudioClip>();
    public List<AudioClip> spawnAmbience = new List<AudioClip>();

    [Header("Ambience Timing")]
    public float minAmbienceDelay = 5f;
    public float maxAmbienceDelay = 15f;

    [Header("Music Fade")]
    public float fadeDuration = 2f;
    public float targetMusicVolume = 1f;

    private Coroutine ambienceRoutine;
    private Coroutine fadeRoutine;

    private AreaType currentArea;
    private bool firstAreaLoad = true;

    void Start()
    {
        //just in case sources is not attatched
        if (musicSource == null || ambienceSource == null)
        {
            AudioSource[] sources = GetComponentsInChildren<AudioSource>();

            if (sources.Length > 0)
                musicSource = sources[0];

            if (sources.Length > 1)
                ambienceSource = sources[1];
        }

        ChangeArea(AreaType.Spawn);
    }

    public void ChangeArea(AreaType newArea)
    {
        if (newArea == currentArea && !firstAreaLoad)
            return;

        currentArea = newArea;

        // Stop previous ambience loop
        if (ambienceRoutine != null)
        {
            StopCoroutine(ambienceRoutine);
        }

        AudioClip newMusic = GetMusicClip(newArea);

        if (firstAreaLoad)
        {
            firstAreaLoad = false;

            musicSource.clip = newMusic;
            musicSource.loop = true;
            musicSource.volume = targetMusicVolume;
            musicSource.Play();
        }
        else
        {
            // Fade for all later transitions
            if (musicSource.clip != newMusic)
            {
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                }

                fadeRoutine = StartCoroutine(FadeMusic(newMusic));
            }
        }

        // Start ambience loop
        ambienceRoutine = StartCoroutine(PlayRandomAmbience());
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // fade out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();

        // change music
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        // fade in
        while (musicSource.volume < targetMusicVolume)
        {
            musicSource.volume += targetMusicVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = targetMusicVolume;
    }

    IEnumerator PlayRandomAmbience()
    {
        while (true)
        {
            float waitTime = Random.Range(minAmbienceDelay, maxAmbienceDelay);
            yield return new WaitForSeconds(waitTime);

            List<AudioClip> currentList = GetAmbienceList(currentArea);

            if (currentList.Count > 0)
            {
                AudioClip randomClip = currentList[Random.Range(0, currentList.Count)];
                ambienceSource.PlayOneShot(randomClip);
            }
        }
    }

    AudioClip GetMusicClip(AreaType area)
    {
        switch (area)
        {
            case AreaType.Forest:
                return forestMusic;

            case AreaType.Mountain:
                return mountainMusic;

            case AreaType.Beach:
                return beachMusic;

            case AreaType.Spawn:
                return spawnMusic;
        }

        return null;
    }

    List<AudioClip> GetAmbienceList(AreaType area)
    {
        switch (area)
        {
            case AreaType.Forest:
                return forestAmbience;

            case AreaType.Mountain:
                return mountainAmbience;

            case AreaType.Beach:
                return beachAmbience;

            case AreaType.Spawn:
                return spawnAmbience;
        }

        return spawnAmbience;
    }
}