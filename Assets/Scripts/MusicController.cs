using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource musicPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        
        Play();
    }

    public void Play()
    {

        // This function reflects changes on sound settings.
        musicPlayer.volume = PreferencesManager.volumeLevel;

        if (PreferencesManager.effectsOn)
        {
            musicPlayer.Play();
        }
        else if (musicPlayer.isPlaying)
        {
            musicPlayer.Stop();
        }
    }
}
