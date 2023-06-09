using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/* ----------------------------------------------------------------------------
 * Class: SoundManager
 * Description: Singleton class that holds and manages the audio files.
 * ---------------------------------------------------------------------------- */
public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Sound Manager in scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("MusicTheme");
    }

    /* ------------------------------------------------------------------------
    * Function: Play
    * Description: Searches the sound array and plays the audio clip associated
    * with the string name
    * ---------------------------------------------------------------------- */
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
            return;
        }
        s.source.Play();
    }

    /* ------------------------------------------------------------------------
    * Function: StopPlay
    * Description: Searches the sound array and stops the currently playing 
    * sound.  Mainly used when soudns are looping.
    * ---------------------------------------------------------------------- */
    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
            return;
        }
        s.source.Stop();
    }

    /* ------------------------------------------------------------------------
    * Function: StopPlay
    * Description: Changes the volume of a sound
    * ---------------------------------------------------------------------- */
    public void ChangeVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
            return;
        }
        s.source.volume = volume;
    }

    /* ------------------------------------------------------------------------
    * Function: PlayUIClick
    * Description: Added a function for easy access to attach to buttons.
    * ---------------------------------------------------------------------- */
    public void PlayUIClick()
    {
        Play("UIClick");
    }
}
