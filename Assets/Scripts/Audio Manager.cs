using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
            // DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
    #endregion

    public AudioSource AudioSource_SE;

    [Tooltip("SE clips")]
    public AudioClip beepbeep;
    public AudioClip SE_demo;

    void Start()
    {
        float vol = PlayerPrefs.GetFloat("Volume", 1);
        AudioSource_SE.volume = vol;
    }

    public float PlaySE(AudioClip clip)
    {
        if (AudioSource_SE.isPlaying)
            AudioSource_SE.Stop();
        AudioSource_SE.PlayOneShot(clip);
        return clip.length;
    }

    // set voluma of se
    public void SetSEVolume(float volume)
    {
        AudioSource_SE.volume = volume;
    }

    // // Play music with a specified clip
    // public void PlayMusic(AudioClip clip)
    // {
    //     musicSource.clip = clip;
    //     musicSource.Play();
    // }

    // // Stop playing music
    // public void StopMusic()
    // {
    //     musicSource.Stop();
    // }

    // // Pause music
    // public void PauseMusic()
    // {
    //     musicSource.Pause();
    // }

    // // Resume playing music
    // public void ResumeMusic()
    // {
    //     musicSource.UnPause();
    // }
}