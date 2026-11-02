using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBack : Singleton<AudioManagerBack>
{
    public AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}