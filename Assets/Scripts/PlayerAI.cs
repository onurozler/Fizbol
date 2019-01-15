using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    // Check if it is target player
    public bool targetPlayer;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "ball")
        {
            if(targetPlayer)
            {
                print("succes");
                // Assign new player to be controlled
                this.gameObject.AddComponent<PlayerControl>();
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                Destroy(this);
            }
            else
            {
                // Start the game again
                print("fail");
            }
        }
    }

    // Catch the Ball
    public void catchTheBall(GameObject ball)
    {
        transform.LookAt(ball.transform);
        transform.position += transform.forward * 4 * Time.deltaTime;
    }
}
