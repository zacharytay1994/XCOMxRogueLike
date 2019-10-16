using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //ManagerScript mScript;
    [Range(0.0f,2.0f)]
    [SerializeField] float musicVolume;
    [Range(0.0f, 2.0f)]
    [SerializeField] float sfxVolume;

    public List<Sound> sounds = new List<Sound>();

    void Awake()
    {
        //mScript = FindObjectOfType<ManagerScript>();
        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.playOnAwake = false;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = sounds.Find(sound => sound.Name == name);

        if (s == null)
        {
            NoFoundSound();
        }
        else
        {
            s.Source.loop = false;

            GameObject playedSound = new GameObject();
            playedSound.AddComponent<AudioSource>();
            AudioSource aus = playedSound.GetComponent<AudioSource>();
            aus.clip = s.Clip;
            playedSound.AddComponent<DeleteSoundScript>();

            Instantiate(playedSound, transform);
        }
    }

    public void PlaySound(string name, float volume) // play sound once
    {
        Sound s = sounds.Find(sound => sound.Name == name);
        if (s == null)
        {
            NoFoundSound();
        }
        else
        {
            s.Volume = volume;

            s.Source.loop = false;

            GameObject playedSound = new GameObject();
            playedSound.AddComponent<AudioSource>();
            AudioSource aus = playedSound.GetComponent<AudioSource>();
            aus.clip = s.Clip;
            playedSound.AddComponent<DeleteSoundScript>();

            Instantiate(playedSound, transform);
        }
    }


    public void PlaySoundOnLoop(string name) // play sound on loop
    {
        Sound s = sounds.Find(sound => sound.Name == name);
        if (s == null)
        {
            NoFoundSound();
        }
        else
        {
            s.Source.loop = true;

            GameObject playedSound = new GameObject();
            playedSound.AddComponent<AudioSource>();
            AudioSource aus = playedSound.GetComponent<AudioSource>();
            aus.clip = s.Clip;
            playedSound.AddComponent<DeleteSoundScript>();

            Instantiate(playedSound, transform);
        }
    }

    public void PlaySoundOnLoop(string name, float volume) // play sound on loop
    {
        Sound s = sounds.Find(sound => sound.Name == name);
        if (s == null)
        {
            NoFoundSound();
        }
        else
        {
            s.Volume = volume;
            s.Source.loop = true;

            GameObject playedSound = new GameObject();
            playedSound.AddComponent<AudioSource>();
            AudioSource aus = playedSound.GetComponent<AudioSource>();
            aus.clip = s.Clip;
            playedSound.AddComponent<DeleteSoundScript>();

            Instantiate(playedSound, transform);
        }
    }

    public void StopSound(string name) // stop sound
    {
        Sound s = sounds.Find(sound => sound.Name == name);

        if (s == null)
        {
            NoFoundSound();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource aus = transform.GetChild(i).GetComponent<AudioSource>();
            if (aus.clip = sounds.Find(sound => sound.Name == name).Clip)
            {
                aus.Stop();
            }
        }

    }

    public void DestroyInstantiatedSound()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource aus = transform.GetChild(i).GetComponent<AudioSource>();
            if (!aus.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public bool CheckSoundActive(string name)
    {
        bool soundActive = false;
        Sound s = sounds.Find(sound => sound.Name == name);

        if (s == null)
        {
            NoFoundSound();
            return soundActive;
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                AudioSource aus = transform.GetChild(i).GetComponent<AudioSource>();
                if (aus.clip == s.Clip)
                {
                    soundActive = true;
                    return soundActive;
                }
                else
                {
                    soundActive = false;
                }
            }
        }

        return soundActive;
    }

    public void Update()
    {
        DestroyInstantiatedSound();
        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource aus = transform.GetChild(i).GetComponent<AudioSource>();
            if (sounds.Find(sound => sound.Clip == aus.clip).SoundType1)
            {
                aus.volume = musicVolume * sounds.Find(sound => sound.Clip == aus.clip).Volume;
            }
            else
            {
                aus.volume = sfxVolume * sounds.Find(sound => sound.Clip == aus.clip).Volume;
            }
        }
        //foreach (Sound s in sounds)
        //{
        //    if(s.SoundType1)
        //    {
        //        s.Source.volume = mScript.musicVolume * s.Volume;
        //    }
        //    else if (!s.SoundType1)
        //    {
        //        s.Source.volume = mScript.soundVolume * s.Volume;
        //    }
        //    //s.source.volume = s.volume;
        //}

    }

    public void NoFoundSound()
    {
        Debug.Log("No sound found");
    }
}
