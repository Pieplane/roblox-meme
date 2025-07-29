using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;
using StarterAssets;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance { get; private set; }
    //public AudioMixerGroup soundsGroup;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private List<Sound> previouslyPlaying = new List<Sound>();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            //return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            // Выбираем группу в зависимости от типа
            switch (s.type)
            {
                case SoundType.Music:
                    s.source.outputAudioMixerGroup = musicGroup;
                    break;
                case SoundType.SFX:
                    s.source.outputAudioMixerGroup = sfxGroup;
                    break;
            }

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void OnEnable()
    {
        ThirdPersonController.OnPlayFootstepSound += PlayFootstep;
        ThirdPersonController.OnPlayLandingSound += PlayFootstep;
    }

    private void OnDisable()
    {
        ThirdPersonController.OnPlayFootstepSound -= PlayFootstep;
        ThirdPersonController.OnPlayLandingSound -= PlayFootstep;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds: " + name + "not found!");
            return;
        }
            
        s.source.Play();
    }
    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Защита от уничтоженного объекта
        if (s.source != null && s.source.gameObject != null)
        {
            s.source.Stop();
        }
    }
    public void OffMusic()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = 0;
        }
    }
    public void OnMusic()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume;
        }
    }
    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            if (!s.ignorePause && s.source != null)
            {
                s.source.Stop();
            }
        }
    }
    public void StopAllExceptBub()
    {
        previouslyPlaying.Clear();

        foreach (Sound s in sounds)
        {
            if (s.source != null && s.source.isPlaying && s.name != "Bub")
            {
                previouslyPlaying.Add(s);
                s.source.Stop();
            }
        }
    }
    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds: " + name + " not found!");
            return false;
        }

        return s.source.isPlaying;
    }
    public void ResumePrevious()
    {
        foreach (Sound s in previouslyPlaying)
        {
            if (s.source != null)
                s.source.Play();
        }

        previouslyPlaying.Clear();
    }
    private void PlayFootstep(AudioClip clip, Vector3 position, float volume)
    {
        PlayClipInSound("Footstep", clip, volume, position);
    }
    public void PlayClipInSound(string soundName, AudioClip clip, float volume = 1f, Vector3? position = null)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null || clip == null)
        {
            Debug.LogWarning("Sound " + soundName + " not found or clip is null.");
            return;
        }

        s.source.clip = clip;
        s.source.volume = volume;

        // Позиционируем, если нужно (3D звук)
        if (position.HasValue)
        {
            s.source.transform.position = position.Value;
            s.source.spatialBlend = 1f;
        }

        s.source.Play();
    }
}
