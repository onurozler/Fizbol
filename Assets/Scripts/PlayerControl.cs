﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //PlayerControl to control selected Player
    float angle = 70 * Mathf.Deg2Rad;
    public void shoot(GameObject ball)
    {
        
        // Add force to ball
        Rigidbody ballRig = ball.GetComponent<Rigidbody>();

        // Remove all sins from the vectors and pray to gods of physics
        Vector3 targetVector = new Vector3(Mathf.Cos(angle)*transform.forward.x, Mathf.Sin(angle), Mathf.Cos(angle)* transform.forward.z);

        ballRig.AddForce(targetVector * 850f);
        
        // Destroy PlayerControl script from selected Player
        Destroy(this);
    }
}