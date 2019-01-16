using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Level Manager Script to control whole objects in the scene.
    [Header("References for GameObjects")]
    //Reference for all Players
    public GameObject allPlayers;

    //Reference for ball
    public GameObject ball;

    //Control for both team
    private GameObject bluePlayers;
    private GameObject redPlayers;

    //Control selected player
    private GameObject selectedPlayer;

    //Target player to get pass from controlled Player
    private GameObject targetPlayer;
    private int target = 1;

    // UI Items
    [Header("UI Items")]
    public Button shootButton;
    
    void Start()
    {
        //Assign teams
        bluePlayers = allPlayers.transform.GetChild(0).gameObject;
        redPlayers = allPlayers.transform.GetChild(1).gameObject;

        //Assign selected player
        selectedPlayer = bluePlayers.transform.GetChild(0).gameObject;

        //Assign target player and Look at selected Player
        targetPlayer = bluePlayers.transform.GetChild(target).gameObject;
        targetPlayer.transform.LookAt(selectedPlayer.transform);

        playerGotTheBall();

    }
    

    public void playerGotTheBall()
    {
        // This function is called from PlayerAI.OnCollisionEnter

        // Show shooting ui
        shootButton.gameObject.SetActive(true);

        //Turn the player towards his next passing mate
        selectedPlayer.transform.LookAt(targetPlayer.transform);

        // Put the ball in front of the player in the direction to shoot, put the ball a little bit ahead so that it doesn't collide with player
        Vector3 newBallPosition = selectedPlayer.transform.position + selectedPlayer.transform.forward * 1.1f;

        // place the ball on the surface
        newBallPosition.y = 1.2f;
        ball.transform.position = newBallPosition;
    }

    public void chooseNewTargetPlayer()
    {
        // This function is called from PlayerAI.OnCollisionEnter

        // New selected Player is target Player
        selectedPlayer = targetPlayer;

        // Assign new targetPlayer
        target++;

        // pass to ball to next blue player until last one, then shoot at goalkeeper of other team.
        if (bluePlayers.transform.childCount > target)
        {
            targetPlayer = bluePlayers.transform.GetChild(target).gameObject;
            
        }
        else
        {
            targetPlayer = redPlayers.transform.GetChild(0).gameObject;
        }

        targetPlayer.AddComponent<PlayerAI>();
        targetPlayer.GetComponent<PlayerAI>().targetPlayer = true;

        // Look at new selected player
        targetPlayer.transform.LookAt(selectedPlayer.transform);
    }

    // This function is called when the shoot button Pressed
    public void isShootPressed()
    {
        // Shoot function in Player Control is called in shoot animation frame when 
        // the ball comes out of the Selected player's foot.

          // Give the ball reference
          selectedPlayer.GetComponent<PlayerControl>().ball = ball;

           // Trigger shoot animation
           selectedPlayer.GetComponent<Animator>().SetBool("isShooting", true);

            // Find closest Player and add PlayerAI Scripts
            GameObject closest = findClosestPlayer(ball);
            closest.AddComponent<PlayerAI>();

            // Makes button Invisible
            shootButton.gameObject.SetActive(false);

            //Setting up animation for closest player to catch the ball
            closest.GetComponent<Animator>().SetBool("isRunning", true);

            // Catch the ball in specific of time
            StartCoroutine(closestPlayerFollowTheBall(closest, 2f));
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

    private IEnumerator closestPlayerFollowTheBall(GameObject closestPlayer, float duration)
    {
        // Call the function every frame till duration time
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            closestPlayer.GetComponent<PlayerAI>().catchTheBall(ball);
            yield return new WaitForFixedUpdate();
        }

        // After player stops following the ball, jump.
        closestPlayer.GetComponent<Rigidbody>().AddForce(Vector3.up * 600);

        // Stop Running and Destroy Player AI from closest player
        closestPlayer.GetComponent<Animator>().SetBool("isRunning", false);
        Destroy(closestPlayer.GetComponent<PlayerAI>());
        
       
    }
}
