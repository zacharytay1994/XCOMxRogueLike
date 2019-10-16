using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField] private string name;
    [SerializeField] private AudioClip clip;
    [Range(0f,1f)]
    [SerializeField] private float volume;
    [Range(0f,1f)]
    [SerializeField] private float pitch;
    [SerializeField] private AudioSource source;
    [SerializeField] private bool SoundType; // true for music, false for sfx

    public string Name { get => name; set => name = value; }
    public AudioClip Clip { get => clip; set => clip = value; }
    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; }
    public AudioSource Source { get => source; set => source = value; }
    public bool SoundType1 { get => SoundType; set => SoundType = value; }
}