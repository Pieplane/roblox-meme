using UnityEngine.Audio;
using UnityEngine;

public enum SoundType
{
    Music,
    SFX,
}
[System.Serializable]
public class Sound
{
    public string name;

    public SoundType type;

    public AudioClip clip;

    public bool ignorePause; // если true Ч звук не выключаетс€ при StopAll

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
