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
    
    void Start()
    {
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
                print("shoott");

                // Last player doesnt look at, Should be fixed

                selectedPlayer.GetComponent<PlayerControl>().canMove = false;
                selectedPlayer.transform.LookAt(redPlayers.transform.GetChild(5));
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

        // Disable him to move
        selectedPlayer.GetComponent<PlayerControl>().canMove = false;

        // Enable him to select target
        targetSelect = true;

        // Put the ball in front of the player in the direction to shoot, put the ball a little bit ahead so that it doesn't collide with player
        Vector3 newBallPosition = selectedPlayer.transform.position + selectedPlayer.transform.forward * 1.1f;

        // place the ball on the surface
        newBallPosition.y = 1.2f;
        ball.transform.position = newBallPosition;
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

    }

    private void chooseTarget()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        // Check if the selected player is blue
        if (hit && hitInfo.transform.gameObject.tag == "blue")
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
            GameObject closest = findClosestPlayer(ball);
            closest.AddComponent<PlayerAI>();
            StartCoroutine(closestPlayerFollowTheBall(closest, dTime, ball));

            //Setting up animation for closest player to catch the ball
            closest.GetComponent<Animator>().SetBool("isRunning", true);

            // Make joystick visible
            joystick.gameObject.SetActive(true);
        }
    }

    private GameObject findClosestPlayer(GameObject ball)
    {
       // Assuming the first is closest
       float closest = Vector3.Distance(redPlayers.transform.GetChild(0).position, ball.transform.position);
       GameObject closestGameObject = redPlayers.transform.GetChild(0).gameObject;

        // Checking if another player is more close
       for (int i = 1; i < redPlayers.transform.childCount; i++)
       {
           float distance = Vector3.Distance(redPlayers.transform.GetChild(i).position, ball.transform.position);
           if(distance < closest)
                {
                    closestGameObject = redPlayers.transform.GetChild(i).gameObject;
                    closest = distance;
                }
        }

       return closestGameObject;
    }

    // Selected player may do dribbling while closest player try to catch the ball.
    private IEnumerator closestPlayerFollowTheBall(GameObject closestPlayer, float duration, GameObject tball)
    {
        // Call the function every frame till duration time
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            dribblingTime.text = "Kalan Zaman : "+ Mathf.Round(startTime - Time.time + 5);
            closestPlayer.GetComponent<PlayerAI>().catchTheBall(tball);
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

        // Put the ball in front of the player in the direction to shoot, put the ball a little bit ahead so that it doesn't collide with player
        Vector3 newBallPosition = selectedPlayer.transform.position + selectedPlayer.transform.forward * 1.1f;

        // place the ball on the surface
        newBallPosition.y = 1.2f;
        ball.transform.position = newBallPosition;

        // Makes button Invisible
        shootButton.gameObject.SetActive(true);
    }

    // Restart the game
    public void restartTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
