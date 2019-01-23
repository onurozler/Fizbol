using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Level Manager Script to control whole objects in the scene.
    [Header("References for GameObjects")]
    //Reference for all Players
    public GameObject allPlayers;
    public PhysicMaterial ballPhysic;

    // Reference for ball
    public GameObject ball;

    // Control for both team
    private GameObject bluePlayers;
    private GameObject redPlayers;

    // Control selected player
    private GameObject selectedPlayer;

    // Target player to get pass from controlled Player
    private GameObject targetPlayer;
    private bool targetSelect = true;

    // Closest Player
    private GameObject closest;

    // Dribbling Timer
    private float dTime = 5.0f;

    // 2D sprites for 2D view
    private GameObject distanceSprite;
    private GameObject angleSprite;

    // UI Items
    [Header("UI Items")]
    public VirtualJoystick joystick;
    public Text dribblingTime;
    public Camera camera2D;
    public Camera mainCamera;
    public GameObject Sprites2D;
    public QuestionManager questionManager;
    
    void Start()
    {
        Physics.gravity = new Vector3(0, -10.0F, 0);
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
    }

    private void Update()
    {
        // For Testing in Editor

        // Select target by Clicking
        if (targetSelect)
        {
            // Check if it is last player to shoot towards Goalkeeper
            if (selectedPlayer.name == "bluePlayer6")
            {
                selectedPlayer.GetComponent<PlayerControl>().canMove = false;

                selectedPlayer.transform.LookAt(redPlayers.transform.GetChild(0));

                // Put ball in front of player
                putBallInfrontOfPlayer();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    chooseTarget();
                }
            }
        }

        /* For touching on mobile
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
        }
        */
    }

    public void playerGotTheBall()
    {
        // This function is called from PlayerAI.OnCollisionEnter


        Mode2D(false);
        Sprites2D.SetActive(false);

        // Destroy closes component
        if (closest != null)
            Destroy(closest.GetComponent<PlayerAI>());

        // New selected Player is target Player
        selectedPlayer = targetPlayer;

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
        
        mainCamera.gameObject.SetActive(true);
        camera2D.gameObject.SetActive(false);
        
    }

    // This function is called when the shoot button Pressed
    public void isShootPressed(int angle)
    {
        // Shoot function in Player Control is called in shoot animation frame when 
        // the ball comes out of the Selected player's foot.
        
        selectedPlayer.GetComponent<PlayerControl>().angle = angle;
        selectedPlayer.GetComponent<PlayerControl>().distance = Vector3.Distance(ball.transform.position, targetPlayer.transform.position);

        // Trigger shoot animation
        selectedPlayer.GetComponent<Animator>().SetBool("isShooting", true);
        
        // Makes virtual joystick Invisible
        joystick.gameObject.SetActive(false);

        // Add bounce to ball
        ball.GetComponent<SphereCollider>().material = ballPhysic;

    }
    public void isShootPressed(int angle, bool isAngleDominant, float multiplier)
    {
        // The incorrect answer alters the multipliers of PlayerControl, which changes the physics input values
        if (isAngleDominant)
        {
            selectedPlayer.GetComponent<PlayerControl>().angleMultiplier = multiplier;
        }
        else
        {
            selectedPlayer.GetComponent<PlayerControl>().distanceMultiplier = multiplier;
        }

        isShootPressed(angle);
    }


    private void chooseTarget()
    {
        // Selected can just pass closer player, it is controlled by gameobject indexes.
        int selectedIndex = selectedPlayer.transform.GetSiblingIndex();
        if (selectedIndex == 1) selectedIndex++;

        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        // Check if the selected player is blue and target index - selected index < 3 (It enables just selected player to pass closer target)
        if (hit && hitInfo.transform.gameObject.tag == "blue" && hitInfo.transform.GetSiblingIndex() - selectedIndex < 3)
        {
            // Assign target player and Look at selected Player
            targetPlayer = hitInfo.transform.gameObject;
            targetPlayer.transform.LookAt(selectedPlayer.transform);
            targetPlayer.AddComponent<PlayerAI>();
            targetPlayer.GetComponent<PlayerAI>().targetPlayer = true;
            targetSelect = false;

            // Start Dribbling Timer
            selectedPlayer.GetComponent<PlayerControl>().canMove = true;

            // Find closest Player and add PlayerAI Scripts
            selectedIndex = selectedPlayer.transform.GetSiblingIndex();
            closest = redPlayers.transform.GetChild(selectedIndex + 1).gameObject;
            closest.AddComponent<PlayerAI>();
            StartCoroutine(closestPlayerFollowTheBall(closest, dTime));

            //Setting up animation for closest player to catch the ball
            closest.GetComponent<Animator>().SetBool("isRunning", true);

            // Make joystick visible
            joystick.gameObject.SetActive(true);
        }
    }

    // Selected player may do dribbling while closest player try to catch the ball.
    private IEnumerator closestPlayerFollowTheBall(GameObject closestPlayer, float duration)
    {
        // Call the function every frame till duration time
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            dribblingTime.text = "Kalan Zaman : "+ Mathf.Round(startTime - Time.time + 5);
            Vector3 middlePoint = (selectedPlayer.transform.position + targetPlayer.transform.position) / 2;
            closestPlayer.GetComponent<PlayerAI>().catchTheBall(middlePoint);
            yield return new WaitForFixedUpdate();
        }

        // Reset text
        dribblingTime.text = "Kalan Zaman : 5";

        // Look at the selected.
        closestPlayer.transform.LookAt(selectedPlayer.transform);

        // Trigger head animation
        StartCoroutine(closest.GetComponent<PlayerAI>().head(ball));

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
        questionManager.AskQuestion();

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
        distanceSprite.transform.GetChild(1).GetComponent<TextMeshPro>().text = "Uzaklik : " + ((distance == 0) ? "x" : angle.ToString());
        distanceSprite.transform.localPosition = new Vector3(distanceSprite.transform.localPosition.x + 0.55f, -10.7f, distanceSprite.transform.localPosition.z - 5f);

        // Angle
        angleSprite.transform.position = ball.transform.position;
        angleSprite.transform.GetChild(1).GetComponent<TextMeshPro>().text = "Açi : " + ((angle==0)?"a":angle.ToString()) + "°";
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

    // Change camera to 2D
    public void changeTo2D(bool is3D)
    {
        if(is3D)
        {
            // Camera position accourding to selected
            camera2D.transform.position = new Vector3(72f,4f,selectedPlayer.transform.position.z + 15f);

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
            }
            else
            {
                if (bluePlayers.transform.GetChild(i).name != selectedPlayer.name && bluePlayers.transform.GetChild(i).name != targetPlayer.name)
                    bluePlayers.transform.GetChild(i).gameObject.SetActive(true);

                if (redPlayers.transform.GetChild(i).name != closest.name)
                    redPlayers.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    // Restart the game
    public void restartTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
