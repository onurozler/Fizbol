using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsController : MonoBehaviour
{
    private AudioSource soundEffectPlayer;
    public AudioClip kick;
    public AudioClip miss;
    public AudioClip goal;


    // Start is called before the first frame update
    void Start()
    {
        soundEffectPlayer = GetComponent<AudioSource>();
         
    }
    
    public void PlayKick() { PlayClip(kick); }
    public void PlayMiss() { PlayClip(miss); }
    public void PlayGoal() { PlayClip(goal); }
    private void PlayClip(AudioClip clip)
    {
        if (PreferencesManager.effectsOn)
        {
            soundEffectPlayer.PlayOneShot(clip, PreferencesManager.volumeLevel);
        }
    }




}
