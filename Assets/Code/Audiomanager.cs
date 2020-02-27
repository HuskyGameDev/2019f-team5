using System;
using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    
    void Start()
    {
        Play("Level Theme");
    }

    //This method goes through the sound array to find the one with the matching name
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        
        if (s == null)
            return;
  
        s.source.Play();
    }
}
