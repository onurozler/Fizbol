using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //PlayerControl to control selected Player

    public void shoot(GameObject ball)
    {
        // Add force to ball
        Rigidbody ballRig = ball.GetComponent<Rigidbody>();
        ballRig.AddForce(transform.forward * 400f);

        // Destroy PlayerControl script from selected Player
        Destroy(this);
    }
}
