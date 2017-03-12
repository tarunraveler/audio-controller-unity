﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour {

    public System.Action<AudioObject> OnFinishedPlaying;
    
#region private fields
    private string _id;
    private AudioClip _clip;
    private AudioSource _source;

    private Coroutine _playingRoutine;
#endregion

#region Public methods and properties
    public string name {
        get {
            return _clip != null ? _clip.name : "NONE";
        }
    }

    public void Setup(string id, AudioClip clip)
    {
        _id = id;
        _clip = clip;
    }

    public void Play(AudioSource source)
    {
        _source = source;
        _source.clip = _clip;
        _source.Play();
        _playingRoutine = StartCoroutine(PlayingRoutine());
    }

    [ContextMenu("Test Play")]
    private void TestPlay()
    {
        Play(_source);
    }
#endregion

    private IEnumerator PlayingRoutine()
    {
        Debug.Log("Playing Started");
        while(true)
        {
            Debug.Log("Checking if it's playing.");
            yield return new WaitForSeconds(0.05f);
            if (!_source.isPlaying)
            {
                Debug.Log("AudioSource Finished Playing");
                break;
            }
        }

        Debug.Log("Not playing anymore");

        if (OnFinishedPlaying != null)
        {
            OnFinishedPlaying(this);
        }

        _source.clip = null;
        _source = null;
        _playingRoutine = null;
    }
}