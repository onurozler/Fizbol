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

    // Dribbling Timer
    private float dTime = 5.0f;

    // UI Items
    [Header("UI Items")]
    public Button shootButton;
    public VirtualJoystick joystick;
    public Text dribblingTime;
    public Camera camera2D;
    public Camera mainCamera;
    public Button button2D; 
    
    void Start()
    {
        // Add Listener to Button 2D
        button2D.onClick.AddListener(() => changeTo2D());

        // Assign teams
        bluePlayers = allPlayers.transform.GetChild(0).gameObject;
        redPlayers = allPlayers.transform.GetChild(1).gameObject;

        // Assign selected player
        selectedPlayer = bluePlayers.transform.GetChild(0).gameObject;
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
                // Players can select 2D
                button2D.gameObject.SetActive(true);

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

        // New selected Player is target Player
        selectedPlayer = targetPlayer;

        // Give ball and Joystick Reference
        selectedPlayer.GetComponent<PlayerControl>().joystick = joystick;
        selectedPlayer.GetComponent<PlayerControl>().ball = ball;

        // Makes button Invisible
        shootButton.gameObject.SetActive(false);

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

        // Disable 2D button && Camera
        button2D.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        camera2D.gameObject.SetActive(false);
    }

    // This function is called when the shoot button Pressed
    public void isShootPressed()
    {
        // Shoot function in Player Control is called in shoot animation frame when 
        // the ball comes out of the Selected player's foot.

           // Trigger shoot animation
           selectedPlayer.GetComponent<Animator>().SetBool("isShooting", true);

            // Makes button Invisible
            shootButton.gameObject.SetActive(false);

            // Makes virtual joystick Invisible
            joystick.gameObject.SetActive(false);

           // Add bounce to ball
           ball.GetComponent<SphereCollider>().material = ballPhysic;

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
            GameObject closest = redPlayers.transform.GetChild(selectedIndex + 1).gameObject;
            closest.AddComponent<PlayerAI>();
            StartCoroutine(closestPlayerFollowTheBall(closest, dTime, ball));

            //Setting up animation for closest player to catch the ball
            closest.GetComponent<Animator>().SetBool("isRunning", true);

            // Make joystick visible
            joystick.gameObject.SetActive(true);
        }
    }

    // Selected player may do dribbling while closest player try to catch the ball.
    private IEnumerator closestPlayerFollowTheBall(GameObject closestPlayer, float duration, GameObject tball)
    {
        // Call the function every frame till duration time
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            dribblingTime.text = "Kalan Zaman : "+ Mathf.Round(startTime - Time.time + 5);
            Vector3 middlePoint = (selectedPlayer.transform.position + targetPlayer.transform.position) / 2;
            closestPlayer.GetComponent<PlayerAI>().catchTheBall(tball,middlePoint);
            yield return new WaitForFixedUpdate();
        }

        // Reset text
        dribblingTime.text = "Kalan Zaman : 5";

        // After player stops following the ball, jump.
        closestPlayer.GetComponent<Animator>().SetBool("isHeading", true);
        //closestPlayer.GetComponent<Rigidbody>().AddForce(Vector3.up * 600);

        // Stop Running and Destroy Player AI from closest player
        closestPlayer.GetComponent<Animator>().SetBool("isRunning", false);
        Destroy(closestPlayer.GetComponent<PlayerAI>());

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

        // Makes button Invisible
        shootButton.gameObject.SetActive(true);

        // Players can select 2D
        button2D.gameObject.SetActive(true);
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
    public void changeTo2D()
    {
        if(button2D.gameObject.name == "2dButton")
        {
            camera2D.transform.position = new Vector3(72f,5f,selectedPlayer.transform.position.z + 15f);

            mainCamera.gameObject.SetActive(false);
            camera2D.gameObject.SetActive(true);
            button2D.gameObject.name = "3dButton";
            targetPlayer.GetComponent<Animator>().SetBool("isTarget",false);
            targetPlayer.transform.rotation = Quaternion.Euler(new Vector3(0, -365.273f,0));
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            camera2D.gameObject.SetActive(false);
            button2D.gameObject.name = "2dButton";
            targetPlayer.GetComponent<Animator>().SetBool("isTarget", true);
            targetPlayer.transform.LookAt(selectedPlayer.transform);

        }

    }

    // Restart the game
    public void restartTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
