using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Level Manager Script to control whole objects in the scene.
    [Header("References for GameObjects")]
    //Reference for all Players
    public GameObject allPlayers;
    public PhysicMaterial ballPhysic;

    // Pitch reference
    public GameObject pitch;

    // Reference for ball
    public GameObject ball;

    // Reference for Material
    public Material selectedMaterial;

    // Control for both team
    private GameObject bluePlayers;
    private GameObject redPlayers;

    // Control selected player
    private GameObject selectedPlayer;

    // Target player to get pass from controlled Player
    private GameObject targetPlayer;
    private bool targetSelect = true;

    //get ball target
    public GameObject ballTarget;

    // Closest Player
    private GameObject closest;

    // Dribbling Timer
    private float dTime = 5f;

    // 2D sprites for 2D view
    private GameObject distanceSprite;
    private GameObject angleSprite;

    // Goal targets, need this reference for toggling visibility
    public GameObject goalTargets;

    // UI Items
    [Header("UI Items")]
    public VirtualJoystick joystick;
    public Text dribblingTime;
    public Camera camera2D;
    public Camera mainCamera;
    public GameObject Sprites2D;
    public QuestionManager questionManager;
    public GameObject menu;
    public GameObject settings;
    public GameObject tutorials;
    public GameObject repeatGame;
    public Button button2D;

    private Quaternion goalkeeperRotation;
    private Vector3 goalKeeperPosition;

    public SoundEffectsController soundEffectPlayer;
    public MusicController musicPlayer;

    private bool watchTutorial = true;
    private bool watchTutorial2 = false;

    private bool is3D = true;


    private void Awake()
    {
        // Check tutorial
        if(PlayerPrefs.GetInt("playerTutorial") == 1)
        {
            GameObject maintutorial = tutorials.transform.GetChild(0).gameObject;
            Toggle notShowAgain = maintutorial.transform.GetChild(2).GetComponent<Toggle>();
            maintutorial.SetActive(true);

            // For continue button
            maintutorial.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            maintutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => maintutorial.SetActive(false));
            maintutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => showTutorial(maintutorial.transform.GetSiblingIndex()+1,notShowAgain.isOn));

            // Exit Button
            maintutorial.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            maintutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => watchTutorial = false);
            maintutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => tutorials.SetActive(false));
            maintutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => PlayerPrefs.SetInt("playerTutorial", notShowAgain.isOn == true ? 0 : 1));

        }
        else
        {
            watchTutorial = false;
            watchTutorial2 = false;
            tutorials.SetActive(false);
        }

    }

    void Start()
    {
        Physics.gravity = new Vector3(0, -5F, 0);
        Time.timeScale = 1f;
        // Assign 2D sprites
        distanceSprite = Sprites2D.transform.GetChild(0).gameObject;
        angleSprite = Sprites2D.transform.GetChild(1).gameObject;

        // Assign teams
        bluePlayers = allPlayers.transform.GetChild(0).gameObject;
        redPlayers = allPlayers.transform.GetChild(1).gameObject;

        // Assign selected player
        selectedPlayer = bluePlayers.transform.GetChild(0).gameObject;
        selectedPlayer.AddComponent<PlayerControl>();
        selectedPlayer.GetComponent<PlayerControl>().joystick = joystick;
        selectedPlayer.GetComponent<PlayerControl>().ball = ball;
        selectedPlayer.GetComponent<PlayerControl>().canMove = false;

        goalkeeperRotation = redPlayers.transform.GetChild(0).rotation;
        goalKeeperPosition = redPlayers.transform.GetChild(0).position;

        // Add Listener to button 2d
        button2D.onClick.RemoveAllListeners();
        button2D.onClick.AddListener(() => SwitchView());

    }

    private void Update()
    {
        // For Testing in Editor

        // Select target by Clicking
        if (targetSelect)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedPlayer.name != "bluePlayer6")
                {
                    chooseTarget();
                }
                else
                {
                    chooseGoalTarget();
                }
                
            }
        }
        
    }

    public void playerGotTheBall()
    {
        // This function is called from PlayerAI.OnCollisionEnter

        button2D.gameObject.SetActive(false);

        // Show the true indication
        questionManager.trueOrFalse.GetComponent<Text>().color = Color.green;
        questionManager.trueOrFalse.GetComponent<Text>().text = LocalizeBase.GetLocalizedString("correct");
        questionManager.trueOrFalse.GetComponent<Animator>().SetTrigger("setAnim");

        //Remove the ball collider from the ball so that when the player is dribbling the ball, the collider on ball doesn't get triggered
        Destroy(ball.GetComponent("BallCollider"));

        Time.timeScale = 1f;

        Mode2D(false);
        Sprites2D.SetActive(false);

        // Hide ball target object
        ballTarget.transform.position = Vector3.zero;
        
        // Remove the PlayerControl script from current shooting player
        Destroy(selectedPlayer.GetComponent<PlayerControl>());

        // Destroy closes component
        if (closest != null)
            Destroy(closest.GetComponent<PlayerAI>());

        // New selected Player is target Player
        selectedPlayer = targetPlayer;



        selectedPlayer.AddComponent<PlayerControl>();

        // Give ball and Joystick Reference
        selectedPlayer.GetComponent<PlayerControl>().joystick = joystick;
        selectedPlayer.GetComponent<PlayerControl>().ball = ball;
        
        // Makes timer visible
        dribblingTime.gameObject.SetActive(true);

        // Close Joystick
        joystick.gameObject.SetActive(false);

        // Reset bounce
        ball.GetComponent<SphereCollider>().material = null;

        // Disable him to move
        selectedPlayer.GetComponent<PlayerControl>().canMove = false;

        // Enable him to select target
        targetSelect = true;

        // Put ball in front of player
        putBallInfrontOfPlayer();

        // Show indication signs for targets.
        showIndicationforTargets();

        // Close indication from selected && add selected indication
        selectedPlayer.transform.GetChild(2).gameObject.SetActive(false);
        selectedPlayer.transform.GetChild(3).gameObject.SetActive(true);

        mainCamera.gameObject.SetActive(true);
        camera2D.gameObject.SetActive(false);

        if (selectedPlayer.name == "bluePlayer6")
        {
            selectedPlayer.transform.LookAt(redPlayers.transform.GetChild(0));
            // Put ball in front of player
            putBallInfrontOfPlayer();

            mainCamera.GetComponent<CameraController>().player = selectedPlayer;
            goalTargets.SetActive(true);

            // Tutorial
            if (watchTutorial2)
            {
                showTutorial(7);
            }
        }


    }

    // This function is called when the shoot button Pressed
    public void isShootPressed(int angle, float angleMultiplier, float distanceMultiplier)
    {
        selectedPlayer.GetComponent<PlayerControl>().angleMultiplier = angleMultiplier;
        selectedPlayer.GetComponent<PlayerControl>().distanceMultiplier = distanceMultiplier;

        // Shoot function in Player Control is called in shoot animation frame when 
        // the ball comes out of the Selected player's foot.


        ball.AddComponent<BallCollider>();

        if (selectedPlayer.name == "bluePlayer6")
        {

            // Initialize goalkeeper animation
            StartCoroutine(redPlayers.transform.GetChild(0).GetComponent<PlayerAI>().goalkeeper(ball));


            // Add special flag to BallCollider so that the ball doesn't respond colliding with the field
            ball.GetComponent<BallCollider>().isForvet = true;

            float distance = Vector3.Distance(ball.transform.position, targetPlayer.transform.position);

            // if the multipliers are 1f it means the answer was correct
            if (distanceMultiplier == 1f && angleMultiplier == 1f)
            {

                // Distance has to be tweaked for the last shot depending on the angle of the shot in order to hit the corner of the goal.
                switch (angle)
                {
                    case 30:
                        distance *= 1.6f;
                        break;
                    case 37:
                        distance *= 1.4f;
                        break;
                    case 45:
                        distance *= 1.25f;
                        break;
                    case 53:
                        distance *= 1.15f;
                        break;
                    case 60:
                        distance *= 1.1f;
                        break;
                }
            }
            else
            {
                // If the last blue player is shooting and the answer is wrong, shoot the ball from the ground directly to the goal
                

                if (distanceMultiplier>1f)
                {
                    angle = 45;
                    distance = 40f; // this determines speed of the shot
                    isAut = true;
                }
                else
                {
                    angle = 1;
                    distance = 1f; // this determines speed of the shot
                }

                
                selectedPlayer.GetComponent<PlayerControl>().angleMultiplier = 1f;
                selectedPlayer.GetComponent<PlayerControl>().distanceMultiplier = 1f;

                soundEffectPlayer.PlayMiss();
            }

           
            selectedPlayer.GetComponent<PlayerControl>().distance = distance;

        }
        else
        {
            selectedPlayer.GetComponent<PlayerControl>().distance = Vector3.Distance(ball.transform.position, targetPlayer.transform.position) - 1;
            if (distanceMultiplier == 1f && angleMultiplier == 1f)
                Destroy(closest.GetComponent<CapsuleCollider>());
        }

        

        selectedPlayer.GetComponent<PlayerControl>().angle = angle;


        // Trigger shoot animation
        selectedPlayer.GetComponent<Animator>().SetBool("isShooting", true);

        
        
        // Makes virtual joystick Invisible
        joystick.gameObject.SetActive(false);

        // Add bounce to ball
        ball.GetComponent<SphereCollider>().material = ballPhysic;

    }

    
    private void chooseGoalTarget()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit && hitInfo.transform.gameObject.tag == "target")
        {
            targetPlayer = hitInfo.transform.gameObject;
            targetPlayer.transform.LookAt(selectedPlayer.transform);

            targetSelect = false;
            goalTargets.SetActive(false);


            GetReadyForQuestion();

            // Tutorial
            if (watchTutorial2)
            {
                showTutorial(8);
            }
        }
    }

    private void chooseTarget()
    {
        // Selected can just pass closer player, it is controlled by gameobject indexes.
        int selectedIndex = selectedPlayer.transform.GetSiblingIndex();
        if (selectedIndex == 1) selectedIndex++;

        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        // Check if the selected player is blue and target index - selected index < 3 (It enables just selected player to pass closer target)
        if (hit && hitInfo.transform.gameObject.tag == "blue" && hitInfo.transform.GetSiblingIndex() - selectedIndex < 3 && selectedPlayer.name != hitInfo.transform.name)
        {
            // Assign target player and Look at selected Player
            targetPlayer = hitInfo.transform.gameObject;
            targetPlayer.transform.LookAt(selectedPlayer.transform);
            targetPlayer.AddComponent<PlayerAI>();
            targetPlayer.GetComponent<PlayerAI>().targetPlayer = true;
            targetPlayer.transform.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().material = selectedMaterial;

            // balltarget indicator
             Vector3 ballTargetPosition = targetPlayer.transform.position + targetPlayer.transform.forward * 0.8f;
            ballTargetPosition.y = 0.4f;
            ballTarget.transform.position = ballTargetPosition;

            targetSelect = false;

            // Start Dribbling Timer
            selectedPlayer.GetComponent<PlayerControl>().canMove = true;

            // Find closest Player and add PlayerAI Scripts
            selectedIndex = selectedPlayer.transform.GetSiblingIndex();
            closest = redPlayers.transform.GetChild(selectedIndex + 1).gameObject;
            closest.AddComponent<PlayerAI>();
            StartCoroutine(closestPlayerFollowTheBall(dTime));

            //Setting up animation for closest player to catch the ball
            closest.GetComponent<Animator>().SetBool("isRunning", true);

            // Make joystick visible
            joystick.gameObject.SetActive(true);

            // Tutorial
            if(watchTutorial)
            {
                Time.timeScale = 0f;
                showTutorial(2);
            }
        }
    }

    // Show Indication Method for indicate targets
    private void showIndicationforTargets()
    {
        int selectedIndex = selectedPlayer.transform.GetSiblingIndex();
        if (selectedIndex % 2 != 0) selectedIndex++;

        if (selectedIndex <= 5)
        {
            if (selectedIndex < 3)
            {
                bluePlayers.transform.GetChild(selectedIndex + 1).GetChild(2).gameObject.SetActive(true);
                bluePlayers.transform.GetChild(selectedIndex + 2).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                bluePlayers.transform.GetChild(selectedIndex + 1).GetChild(2).gameObject.SetActive(true);
            }
        }
    }


    // Selected player may do dribbling while closest player try to catch the ball.
    private IEnumerator closestPlayerFollowTheBall(float duration)
    {
        // Call the function every frame till duration time
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            dribblingTime.text = LocalizeBase.GetLocalizedString("time_left") + Mathf.Round(startTime - Time.time + 5);
            Vector3 middlePoint = (selectedPlayer.transform.position + targetPlayer.transform.position) / 2;
            closest.GetComponent<PlayerAI>().catchTheBall(middlePoint);
            yield return new WaitForFixedUpdate();
        }

        GetReadyForQuestion();
        button2D.gameObject.SetActive(true);

        // Tutorial
        if (watchTutorial)
        {
            Time.timeScale = 0f;
            showTutorial(3);
            watchTutorial = false;
            watchTutorial2 = true;
        }
    }

    private void GetReadyForQuestion()
    {
        // Reset text
        dribblingTime.text = LocalizeBase.GetLocalizedString("time_left")+5;

        // Look at the selected.
        closest.transform.LookAt(selectedPlayer.transform);

        // Trigger head animation
        if (selectedPlayer.name != "bluePlayer6")
        {
            StartCoroutine(closest.GetComponent<PlayerAI>().head(ball));
        }

        

        // After time finishes
        // Movement disabled and ready for shoot

        // Disable him to move
        selectedPlayer.GetComponent<PlayerControl>().canMove = false;

        //Turn the player towards his next passing mate
        selectedPlayer.transform.LookAt(targetPlayer.transform);

        // Ready for shoot
        selectedPlayer.GetComponent<Animator>().SetBool("readyForShoot", true);

        // Make invisible the Text
        dribblingTime.gameObject.SetActive(false);

        // Put ball in front of player
        putBallInfrontOfPlayer();

        // Hide joystick
        joystick.gameObject.SetActive(false);

        // Brings up the question UI and holds the control until player answeres correctly
        questionManager.AskQuestion(false);

        Sprites2D.SetActive(false);
    }

    public void DisplayInfoSprites(int angle, int distance)
    {
        // Put 2D Sprites
        // Distance
        Transform distanceS = distanceSprite.transform.GetChild(0);
        distanceSprite.transform.position = (selectedPlayer.transform.position + targetPlayer.transform.position) / 2;
        distanceS.LookAt(targetPlayer.transform);
        distanceS.localEulerAngles = new Vector3(0, distanceS.localEulerAngles.y + 90f, 0); ;
        distanceS.GetComponent<SpriteRenderer>().size = new Vector2(Vector3.Distance(selectedPlayer.transform.position, targetPlayer.transform.position), 0.2f);
        distanceSprite.transform.GetChild(1).GetComponent<TextMesh>().text = LocalizeBase.GetLocalizedString("distance");
        distanceSprite.transform.localPosition = new Vector3(distanceSprite.transform.localPosition.x + 0.55f, -11.02f, distanceSprite.transform.localPosition.z - 5f);

        // Angle
        angleSprite.transform.position = ball.transform.position;
        angleSprite.transform.GetChild(1).GetComponent<TextMesh>().text = LocalizeBase.GetLocalizedString("angle") + ((angle==0)?"a":angle.ToString()) + "°";
        angleSprite.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, (angle == 0) ? 60 : angle);
    }

    // Put ball in front of player
    private void putBallInfrontOfPlayer()
    {
        // Put the ball in front of the player in the direction to shoot, put the ball a little bit ahead so that it doesn't collide with player
        Vector3 newBallPosition = selectedPlayer.transform.position + selectedPlayer.transform.forward * 1.1f;

        // place the ball on the surface
        newBallPosition.y = 1.2f;
        ball.transform.position = newBallPosition;
    }

    // switch View
    public void SwitchView()
    {
        redPlayers.transform.GetChild(0).rotation = goalkeeperRotation;
        redPlayers.transform.GetChild(0).position = goalKeeperPosition;
        changeTo2D(is3D);
        is3D = !is3D;
    }

    // Change camera to 2D
    public void changeTo2D(bool is3D)
    {
        if(is3D)
        {
            // Camera position accourding to selected
            camera2D.transform.position = new Vector3(72f,4f,selectedPlayer.transform.position.z + 24f);

            // Settings for 2D
            mainCamera.gameObject.SetActive(false);
            camera2D.gameObject.SetActive(true);
            Mode2D(true);

            // Setting for 2D Sprites
            Sprites2D.SetActive(true);
        }
        else
        {
            // Settings for 3D
            mainCamera.gameObject.SetActive(true);
            camera2D.gameObject.SetActive(false);
            Mode2D(false);
            Sprites2D.SetActive(false);

        }

    }

    // 2D Mode settings, game objects become invisible to prevent conflicts except target,selected and closest.
    private void Mode2D(bool check)
    {
        for(int i = 0 ; i < 6; i++)
        {
            if (check)
            {
                if (bluePlayers.transform.GetChild(i).name != selectedPlayer.name && bluePlayers.transform.GetChild(i).name != targetPlayer.name)
                    bluePlayers.transform.GetChild(i).gameObject.SetActive(false);

                if (redPlayers.transform.GetChild(i).name != closest.name)
                    redPlayers.transform.GetChild(i).gameObject.SetActive(false);

                redPlayers.transform.GetChild(0).gameObject.SetActive(true);

                if(i < 3)
                pitch.transform.GetChild(i + 6).gameObject.SetActive(false);
            }
            else
            {
                if (bluePlayers.transform.GetChild(i).name != selectedPlayer.name && bluePlayers.transform.GetChild(i).name != targetPlayer.name)
                    bluePlayers.transform.GetChild(i).gameObject.SetActive(true);

                if (redPlayers.transform.GetChild(i).name != closest.name)
                    redPlayers.transform.GetChild(i).gameObject.SetActive(true);

                if (i < 3)
                    pitch.transform.GetChild(i + 6).gameObject.SetActive(true);
            }
        }
    }


    // Prevent position issue after miss
    private bool isAut = false;
    // Restart the game
    public void restartLastPosition()
    {
        // Remove the ball collider from the ball so that when the player is dribbling the ball, the collider on ball doesn't get triggered
        Destroy(ball.GetComponent("BallCollider"));

        putBallInfrontOfPlayer();


        // This is needed to trigger the closest player to jump again 
        closestPlayerFollowTheBall(0);

        Debug.Log("restart");
        questionManager.AskQuestion(true);

        // For resetting goalkeeper Position and animation
        if(selectedPlayer.name == "bluePlayer6")
        {
            // Due to an unknown cause, the gk does not reset his ol position.
            // Because of this, we had to fix it by hardcoding some predetermined values.
            redPlayers.transform.GetChild(0).rotation = goalkeeperRotation;
            
            if (redPlayers.transform.GetChild(0).GetComponent<Animator>().GetBool("isSaveLeft"))
            {
                redPlayers.transform.GetChild(0).GetComponent<Animator>().SetBool("isSaveLeft", false);
                
                redPlayers.transform.GetChild(0).position = goalKeeperPosition + new Vector3(isAut==false ? -2.5f : 0f,0f,0f);
            }
            else
            {
                redPlayers.transform.GetChild(0).GetComponent<Animator>().SetBool("isSaveRight", false);
                redPlayers.transform.GetChild(0).position = goalKeeperPosition + new Vector3(isAut == false ? 2.5f : 0f, 0f, 0f);
            }

            isAut = false;
        }
    }


    // Open Menu
    public void openMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }
    // Close Menu
    public void closeMenu()
    {
        Time.timeScale = 1f;
        
        menu.SetActive(false);
    }

    // Back to Menu
    public void backToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene2");
    }

    // Open Settings
    public void openSettings()
    {
        settings.SetActive(true);

        // Set settings
        settings.transform.GetChild(1).GetChild(1).GetComponent<Toggle>().isOn = PreferencesManager.effectsOn;
        settings.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = PreferencesManager.volumeLevel;
        settings.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = true;

        if(PreferencesManager.quality == 1)
            settings.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<Toggle>().isOn = true;
        else if (PreferencesManager.quality == 2)
            settings.transform.GetChild(3).GetChild(1).GetChild(2).GetComponent<Toggle>().isOn = true;
    }
    // Close Settings
    public void closeSettings()
    {
        settings.SetActive(false);

        float volume = settings.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value;

        // Set users settings to player Prefs
        bool effect = settings.transform.GetChild(1).GetChild(1).GetComponent<Toggle>().isOn ? true : false;
        int quality = 2;

        if (settings.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Toggle>().isOn) quality = 0;
        else if (settings.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<Toggle>().isOn) quality = 1;

        PlayerPrefs.SetString("playerSettings", PreferencesManager.musicOn.ToString() + ";" + effect.ToString() + ";" + volume.ToString() + ";" + quality);

        PreferencesManager.volumeLevel = volume;
        PreferencesManager.quality = quality;
        PreferencesManager.effectsOn = effect;

        QualitySettings.SetQualityLevel(quality * 2);
        musicPlayer.Play();

    }

    // Load Game again
    public void loadGameAgain()
    {
        SceneManager.LoadScene("Scene1");
    }

    private void showTutorial(int num, bool notShowAgain)
    {
        if (notShowAgain) PlayerPrefs.SetInt("playerTutorial", 0);

        GameObject tutorial = tutorials.transform.GetChild(num).gameObject;
        tutorial.SetActive(true);

        // For continue button
        tutorial.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => tutorial.SetActive(false));

        // For Exit button
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => watchTutorial = false);
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => tutorials.SetActive(false));

    }
    private void showTutorial(int num)
    {

        GameObject tutorial = tutorials.transform.GetChild(num).gameObject;
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        tutorial.SetActive(true);

        if (num >= 3 && num < 6)
        {
            // For continue button
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => tutorial.SetActive(false));
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => showTutorial(num + 1));

            questionManager.questionPanel.SetActive(false);
            tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => questionManager.questionPanel.SetActive(true));
        }
        else
        {
            // For continue button
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => tutorial.SetActive(false));
            tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => Time.timeScale = 1f);
            if(num == 8) questionManager.questionPanel.SetActive(false);
            if (num == 6 ||num == 8)
            {
                tutorial.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => questionManager.questionPanel.SetActive(true));
                tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => questionManager.questionPanel.SetActive(true));
            }
        }

        // For Exit button

        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => watchTutorial = false);
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => watchTutorial2 = false);
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => Time.timeScale=1f);
        tutorial.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => tutorials.SetActive(false));

    }
}
