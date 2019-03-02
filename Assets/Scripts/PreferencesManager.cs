using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferencesManager : MonoBehaviour
{

    public static bool musicOn = false;
    public static bool effectsOn = false;
    public static float volumeLevel = 0f;
    public static int quality = 0;

    void Start()
    {
        
    }

    private void Awake()
    {
        string[] settings = PlayerPrefs.GetString("playerSettings").Split(';');
        musicOn = bool.Parse(settings[0]);
        effectsOn = bool.Parse(settings[1]);
        volumeLevel = float.Parse(settings[2]);
        quality = int.Parse(settings[3]);
        Debug.Log("Read settings");
    }
}
