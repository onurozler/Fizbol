using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    // References
    public Camera cam;
    public GameObject[] screens;

    // Music References and controls
    public AudioClip audios;
    public AudioClip musicClip;
    public AudioSource aSource;
    public AudioSource MusicController;
    private bool effectsOn;
    private bool musicOn;
    private float volumeOn;

    // Settings
    public Toggle[] toggles;
    public Slider volumeSlider;

    private float timeLeft = 5.0f;

    //This delimeter is for splitting date string and it is platform dependant
#if UNITY_ANDROID
    private char delimeter = '/';

#else
    private char delimeter = '.';

#endif

    // New register for everyday
    private string newRegister = System.DateTime.Now.ToString("dd/MM/yyyy") + "-0;0";

    private void Awake()
    {
        // Set player statisctic if the player opens the game first time
        // User statistic will be saved as 10.06.2019-15;5, 11.06.2019-25;15 (Date - True Answer ; Wrong Answer , second record..)
        if (!PlayerPrefs.HasKey("playerStatistic"))
        {
            PlayerPrefs.SetString("playerStatistic", newRegister);

            // Sound = true, Effects = true, Volume = 1, QualityToggle = 2 
            PlayerPrefs.SetString("playerSettings","true;true;0.5;2");

            // Set Player Tutorial = On
            PlayerPrefs.SetInt("playerTutorial", 1);
        }
    }

    private string[] lastDate;
    private string[] getWhole;
    private void Start()
    {
        // Get current date
        string newDate = System.DateTime.Now.ToString("dd/MM/yyyy");
   
        // Add new Date if it today is different
        string lastStatistic = PlayerPrefs.GetString("playerStatistic");
        // Get last record
        getWhole = lastStatistic.Split(',');
        // Get last date
        lastDate = getWhole[getWhole.Length - 1].Split('-');

        if (lastDate[0] != newDate)
        {
            PlayerPrefs.SetString("playerStatistic", lastStatistic+","+newRegister);

            // Add new Date if it today is different
            lastStatistic = PlayerPrefs.GetString("playerStatistic");
            // Get last record
            getWhole = lastStatistic.Split(',');
            // Get last date
            lastDate = getWhole[getWhole.Length - 1].Split('-');
        }

        // Get Settings from PlayerPrefs
        string[] settings = PlayerPrefs.GetString("playerSettings").Split(';');
        setSettings(bool.Parse(settings[0]), bool.Parse(settings[1]), float.Parse(settings[2]), int.Parse(settings[3]));
        musicOn = bool.Parse(settings[0]);
        effectsOn = bool.Parse(settings[1]);
        volumeOn = float.Parse(settings[2]);
        MusicController.volume = volumeOn;


        // Load Graphics
        loadGraphs();

        // Start Menu Music
        if (musicOn)
        {
            MusicController.clip = musicClip;
            MusicController.Play();
        }
    }

    void Update()
    {
        // Camera position in the beginning of game
        
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            cam.transform.position += transform.up * Time.deltaTime * 2f;
        }
    }

    // Click Start Button
    public void startGame()
    {
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
        SceneManager.LoadScene("Scene1");
    }

    // Click Statistic Button
    public void openStatistic()
    {
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
        screens[0].SetActive(true);
    }
    public void closeStatistic()
    {
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
        screens[0].SetActive(false);
    }

    // Click Settings Button
    public void openSettings() {
        screens[1].SetActive(true);
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
    }
    public void closeSettings() {
        screens[1].SetActive(false);

        // Set users settings to player Prefs
        bool music = toggles[3].isOn ? true : false;
        bool effect = toggles[4].isOn ? true : false;
        int quality = 2;

        if (toggles[0].isOn) quality = 0;
        else if (toggles[1].isOn) quality = 1;

        setSettings(music,effect ,volumeSlider.value, quality);
        PlayerPrefs.SetString("playerSettings", music.ToString()+";"+effect.ToString()+";"+volumeSlider.value.ToString()+";"+quality);
        musicOn = music;
        effectsOn = effect;
        volumeOn = volumeSlider.value;
        MusicController.volume = volumeOn;

        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);

        // Play or Stop Music
        if (!musicOn) MusicController.Pause();
        else  MusicController.UnPause() ;

        if (musicOn && !MusicController.isPlaying)
        {
            MusicController.clip = musicClip;
            MusicController.Play();
        }
          
    }

    // Click Credits Button
    public void openCredits()
    {
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
        screens[2].SetActive(true);

        // Links to developers Linkedin or Mail
        Button onurLinkedin = screens[2].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Button>();
        Button onurMail = screens[2].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Button>();

        Button sinanLinkedin = screens[2].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>();
        Button sinanMail = screens[2].transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Button>();

        // Add listeners to links
        onurLinkedin.onClick.RemoveAllListeners();
        onurLinkedin.onClick.AddListener(() => Application.OpenURL("https://www.linkedin.com/in/onurozler/"));

        onurMail.onClick.RemoveAllListeners();
        onurMail.onClick.AddListener(() => Application.OpenURL("https://mail.google.com/mail/u/0/?view=cm&fs=1&to=onurozlerr1997@gmail.com&su=Fizbol%20Hakkinda&body=Onerilerinizi%20ve%20geridonuslerinizi%20yazabilirsiniz.&tf=1"));

        sinanLinkedin.onClick.RemoveAllListeners();
        sinanLinkedin.onClick.AddListener(() => Application.OpenURL("https://www.linkedin.com/in/sinansakaoglu/"));

        sinanMail.onClick.RemoveAllListeners();
        sinanMail.onClick.AddListener(() => Application.OpenURL("https://mail.google.com/mail/u/0/?view=cm&fs=1&to=sinanssakaoglu@gmail.com&su=Fizbol%20Hakkinda&body=Onerilerinizi%20ve%20geridonuslerinizi%20yazabilirsiniz.&tf=1"));

    }
    public void closeCredits()
    {
        // Play button click
        if (effectsOn) aSource.PlayOneShot(audios, volumeOn);
        screens[2].SetActive(false);
    }

    private string monthName(int i)
    {
        if (i == 1) return LocalizeBase.GetLocalizedString("january");
        else if (i == 2) return LocalizeBase.GetLocalizedString("febuary");
        else if (i == 3) return LocalizeBase.GetLocalizedString("march");
        else if (i == 4) return LocalizeBase.GetLocalizedString("april");
        else if (i == 5) return LocalizeBase.GetLocalizedString("may");
        else if (i == 6) return LocalizeBase.GetLocalizedString("june");
        else if (i == 7) return LocalizeBase.GetLocalizedString("july");
        else if (i == 8) return LocalizeBase.GetLocalizedString("august");
        else if (i == 9) return LocalizeBase.GetLocalizedString("september");
        else if (i == 10) return LocalizeBase.GetLocalizedString("october");
        else if (i == 11) return LocalizeBase.GetLocalizedString("november");
        else return LocalizeBase.GetLocalizedString("december");
    }

    private void loadGraphs()
    {
        // Assign text
        Text dateText = screens[0].transform.GetChild(1).GetComponent<Text>();
        Text succesfulText = screens[0].transform.GetChild(3).GetComponent<Text>();
        Text failText = screens[0].transform.GetChild(4).GetComponent<Text>();
        // Assign Graphs
        Image succesful = screens[0].transform.GetChild(2).GetChild(1).GetComponent<Image>();
        GameObject chart = screens[0].transform.GetChild(5).GetChild(0).gameObject;
        // Find true and false answers
        string[] answers = lastDate[1].Split(';');
        // Set last record values
        dateText.text = LocalizeBase.GetLocalizedString("stats_todays_data") + lastDate[0];
        succesfulText.text = LocalizeBase.GetLocalizedString("stats_correct_answers") + answers[0];
        failText.text = LocalizeBase.GetLocalizedString("stats_incorrect_answers") + answers[1];
        // Set circle graph

        // Find percentage
        float value = float.Parse(answers[0]) * 100 / (float.Parse(answers[0]) + float.Parse(answers[1]));
        // Set to graph
        succesful.fillAmount = value / 100;


        // Set chart graph

        // Get bar object from prefabs folder
        GameObject barObject = Resources.Load("Prefabs/barObject") as GameObject;
        // Get This month
        string today = System.DateTime.Now.ToString("dd/MM/yyyy");
        string[] thisMonth = today.Split(delimeter);
        chart.transform.parent.GetChild(4).GetComponent<Text>().text = monthName(int.Parse(thisMonth[1])) + LocalizeBase.GetLocalizedString("stats_months_data");

        for (int i = 0; i < getWhole.Length; i++)
        {
            string[] getMonth = getWhole[i].Split(delimeter);
            string[] getAnswers = getWhole[i].Split('-');
            string[] findAnswer = getAnswers[1].Split(';');
            if (thisMonth[1] == getMonth[1])
            {
                barObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(10f, (float.Parse(findAnswer[0]) * 5) >= 173f ? 173f : (float.Parse(findAnswer[0]) * 5));
                barObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(10f, (float.Parse(findAnswer[1]) * 5) >= 173f ? 173f : (float.Parse(findAnswer[1]) * 5));
                barObject.transform.GetChild(2).GetComponent<Text>().text = getMonth[0];
                Instantiate(barObject, chart.transform);
            }
        }
    }

    private void setSettings(bool music,bool effect,float volume, int quality)
    {
        // Set Quality
        toggles[quality].isOn = true;
        QualitySettings.SetQualityLevel(quality*2);

        // Set Music
        if (music) toggles[3].isOn = true;
        else toggles[3].isOn = false;

        // Set Volume
        if (effect) toggles[4].isOn = true;
        else toggles[4].isOn = false;

        // Set volume
        volumeSlider.value = volume;

    }


}
