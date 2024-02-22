using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Sound[] sounds;

    private Sound battleThemeIntro;
    private Sound battleThemeLoop;

    private Sound bossBattleThemeIntro;
    private Sound bossBattleThemeLoop;

    // The static instance that can be accessed from anywhere
    private static AudioManager _instance;

    // The public property to get the instance
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    // If the instance doesn't exist, create a new GameObject with the script attached
                    GameObject singletonObject = new GameObject(typeof(AudioManager).Name);
                    _instance = singletonObject.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {

        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private Sound FindSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
                return s;
        }
        return null;
    }

    public void Play(string sound)
    {
        FindSound(sound).source.Play();
    }

    public void OnlyPlayAfterSoundEnds(string sound)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == sound && !s.source.isPlaying)
                s.source.Play();
        }
    }

    public void Stop(string sound)
    {
        FindSound(sound).source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.Stop();
    }

    public void Pause(string sound)
    {
        FindSound(sound).source.Pause();
    }

    public void Unpause(string sound)
    {
        FindSound(sound).source.UnPause();
    }

    public void PauseAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.Pause();
    }

    public void UnpauseAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.UnPause();
    }

    public bool CheckIfSoundPlaying(string sound)
    {
        return FindSound(sound).source.isPlaying;
    }

    public void FadeAllSound(bool fadeIn, float duration, float targetVolume)
    {
        foreach(Sound s in sounds)
        {
            StartCoroutine(FadeOutSound(fadeIn, s, duration, targetVolume));
        }
    }

    public void FadeSound(bool fadeIn, string sound, float duration, float targetVolume)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == sound)
                StartCoroutine(FadeOutSound(fadeIn, s, duration, targetVolume));
        }
    }

    private void ResetVolumeOfSound(Sound sound)
    {
        foreach(Sound s in sounds)
        {
            if (s.name == sound.name)
                s.source.volume = 1;
        }
    }

    public float GetClipDuration(string sound)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == sound)
                return s.clip.length;
        }

        return 0;
    }

    public void PlayAfterDelay(float delay, string sound)
    {
        StartCoroutine(PlayAfterDelay(delay, FindSound(sound)));
    }

    public void PlayWithRandomPitch(string sound, float minPitch, float maxPitch)
    {
        float random = Random.Range(minPitch, maxPitch);
        Sound soundToPlay = FindSound(sound);

        soundToPlay.pitch = random;
        soundToPlay.source.Play();
    }

    public void SetBattleThemeStatus(bool play)
    {
        if (play && !battleThemeIntro.source.isPlaying && !battleThemeLoop.source.isPlaying)
        {
            battleThemeIntro.source.Play();
            StartCoroutine(PlayAfterDelay(battleThemeIntro.clip.length, battleThemeLoop));
        }
        else if (!play)
        {
            battleThemeIntro.source.Stop();
            battleThemeLoop.source.Stop();
        }
    }

    public void SetBossBattleThemeStatus(bool play)
    {
        if (play && !bossBattleThemeIntro.source.isPlaying && !bossBattleThemeLoop.source.isPlaying)
        {
            bossBattleThemeIntro.source.Play();
            StartCoroutine(PlayAfterDelay(bossBattleThemeIntro.clip.length, bossBattleThemeLoop));
        }
        else if (!play)
        {
            bossBattleThemeIntro.source.Stop();
            bossBattleThemeLoop.source.Stop();
        }
    }

    private IEnumerator FadeOutSound(bool fadeIn, Sound sound, float fadeDuration, float targetVolume)
    {
        float time = 0f;
        float startVolume = sound.source.volume;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            if (!fadeIn)
                sound.source.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            else
                sound.source.volume = Mathf.Lerp(0, targetVolume, time / fadeDuration);
            yield return null;
        }

        sound.source.Stop();
        ResetVolumeOfSound(sound);

        yield break;
    }

    private IEnumerator PlayAfterDelay(float delay, Sound sound)
    {
        yield return new WaitForSeconds(delay);

        sound.source.Play();
    }
}