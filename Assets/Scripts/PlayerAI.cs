using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    // Check if it is target player
    public bool targetPlayer;
    
    private LevelManager levelManager;

    // Animation Control
    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        // Setting up Target Player Animation
        if(targetPlayer)
        {
            anim.SetBool("isTarget", true);
        }
    }


    // Trigger goalkeeper animation
    public IEnumerator goalkeeper(GameObject ball)
    {
        Vector3 ballV;
        Vector3 goalkeeperV;
        do
        {
            ballV = new Vector3(ball.transform.position.x, 5, ball.transform.position.z);
            goalkeeperV = new Vector3(transform.position.x, 5, transform.position.z);
            yield return new WaitForFixedUpdate();
        } while (Vector3.Distance(ballV, goalkeeperV) >= 15f);

        if (goalkeeperV.x < ballV.x)
            transform.GetComponent<Animator>().SetBool("isSaveRight", true);
        else
            transform.GetComponent<Animator>().SetBool("isSaveLeft", true);
    }

    // Trigger head animation
    public IEnumerator head(GameObject ball)
    {
        transform.GetComponent<Animator>().SetBool("isRunning", false);
        Vector3 ballV;
        Vector3 closestV;
        do
        {
            ballV = new Vector3(ball.transform.position.x, 5, ball.transform.position.z);
            closestV = new Vector3(transform.position.x, 5, transform.position.z);
            yield return new WaitForFixedUpdate();
        } while (Vector3.Distance(ballV, closestV) >= 5.0f);

        
        transform.GetComponent<Animator>().SetBool("isHeading",true);
    }

    // Catch the Ball
    public void catchTheBall(Vector3 middlePoint)
    {
        transform.GetComponent<Animator>().SetBool("isRunning", true);

        //When the threshold gets below .5f, the never stops running and twitches
        if (Vector3.Distance(transform.position, middlePoint) > .5f )
        {
            transform.LookAt(middlePoint);
            transform.position += transform.forward * 4 * Time.deltaTime;  
        }
        else
        {
            transform.GetComponent<Animator>().SetBool("isRunning", false);

        }

        /*
      else
      {
              // We keep the y axis of the looking direction same so that the player doesn't flip

         Vector3 lookingDirection = new Vector3(ball.transform.position.x, 3, ball.transform.position.z);
         transform.LookAt(lookingDirection);
         transform.position += transform.forward * 7 * Time.deltaTime;
      }
      */
    }
}
