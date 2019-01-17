using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //PlayerControl to control selected Player
    float angle = 70 * Mathf.Deg2Rad;
    public GameObject ball;
    public VirtualJoystick joystick;

    // Movement Check
    public bool canMove;

    //Animation Control
    private Animator anim;

    private void Start()
    {
        // Setting up shooter animation
        anim = this.GetComponent<Animator>();
        anim.SetBool("isShooter", true);
    }

    // Character Movement
    private void Update()
    {
        if (canMove)
        {
            // Move Character
            transform.position += new Vector3(joystick.Horizantal() / 10, 0, 0);
            transform.position += new Vector3(0, 0, joystick.Vertical() / 10);

            // Move Ball
            ball.transform.position += new Vector3(joystick.Horizantal() / 10, 0, 0);
            ball.transform.position += new Vector3(0, 0, joystick.Vertical() / 10);

            // Rotate Character and Ball according to Joystick direction
            if (joystick.Horizantal() != 0 || joystick.Vertical() != 0)
            {
                float angle = Mathf.Atan2(joystick.Horizantal(), joystick.Vertical()) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }
            Vector3 newBallPosition = transform.position + transform.forward * 1.1f;

            // place the ball in front of player
            newBallPosition.y = 1.2f;
            ball.transform.position = newBallPosition;

            // Set up animation according to movement
            anim.SetFloat("Horizantal", joystick.Horizantal());
            anim.SetFloat("Vertical", joystick.Vertical());
        }

       
    }

    private void shoot()
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
