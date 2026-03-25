using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance;
  private AudioSource systemSource;
  private List<AudioSource> activeSources;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

//Funções de gerenciamento de audio
  public void PlaySound(AudioClip clip)
  {
    systemSource.Stop();
    systemSource.clip = clip;
    systemSource.Play();
  }

  public void Stopsound()
  {
    systemSource.Stop();
  }

  public void Pausesound()
  {
    systemSource.Pause();
  }

  public void Resumesound()
  {
    systemSource.UnPause();
  }

  public void PlayOneShot(AudioClip clip)
  {
    systemSource.PlayOneShot(clip);
  }

  //Funções de gerenciamento de audio 3D
  public void PlaySound(AudioClip clip, AudioSource source)
  {
    if (!activeSources.Contains(source)) activeSources.Add(source);
    source.Stop();
    source.clip = clip;
    source.Play();
  }

  public void StopSound3d(AudioSource source)
  {
    systemSource.Stop();
    activeSources.Remove(source);
  }
  public void PauseSound3d(AudioSource source)
  {
    systemSource.Pause();
  }
  public void ResumeSound3d(AudioSource source)
  {
    systemSource.UnPause();
    
  }
}
