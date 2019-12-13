﻿using Fumbbl;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, AudioClip> SoundEffectClips;

    public AudioSource SoundEffectSource;



    ///////////////////////////////////////////////////////////////////////////
    //  MONOBEHAVIOUR METHODS  ////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    private void OnDisable()
    {
        FFB.Instance.OnSound -= Play;
    }

    private void Start()
    {
        FFB.Instance.OnSound += Play;
        Object[] AudioFiles;
        AudioFiles = Resources.LoadAll("Audio", typeof(AudioClip));

        SoundEffectClips = new Dictionary<string, AudioClip>();

        foreach(AudioClip file in AudioFiles)
        {
            SoundEffectClips.Add(file.name, file);
        }
    }



    ///////////////////////////////////////////////////////////////////////////
    //  CUSTOM METHODS  ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    public void Play(string sound)
    {
        AudioClip clip = SoundEffectClips[sound];
        if(clip != null && !FFB.Instance.Settings.Sound.Mute)
        {
            SoundEffectSource.PlayOneShot(clip, FFB.Instance.Settings.Sound.GlobalVolume);
        }
    }
}
