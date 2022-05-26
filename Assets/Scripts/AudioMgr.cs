using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    public AudioClip[] audioClips;

    public AudioSource bgmSource;
    public AudioSource effectSource;

    public void PlayEffectSound()
    {
        effectSource.Play();
    }

}
