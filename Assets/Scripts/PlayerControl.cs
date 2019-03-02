using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //PlayerControl to control selected Player
    public int angle = 60;
    public float distance;
    public float distanceMultiplier = 1f;
    public float angleMultiplier = 1f;

    public GameObject ball;
    public VirtualJoystick joystick;

    // Movement Check
    public bool canMove;

    //Animation Control
    private Animator anim;

    private SoundEffectsController soundEffectPlayer;

    private void Start()
    {
        // Setting up shooter animation
        anim = this.GetComponent<Animator>();
        anim.SetBool("isShooter", true);

        soundEffectPlayer = GameObject.Find("SoundEffectController").GetComponent<SoundEffectsController>();
    }

    // Character Movement
    private void Update()
    {
        if (canMove)
        {
            // Keep the player inside pitch

            if (!(joystick.Horizantal() > 0 && transform.position.x > 58.5f || joystick.Horizantal() < 0 && transform.position.x < -58.5f
    || joystick.Vertical() < 0 && transform.position.z < -103.5f))
            {

                // This check prevents errors when game is resumed from paused menu.
                if (Time.deltaTime == 0)
                {
                    return;
                }

                // Move Character
                transform.position += new Vector3(0, 0, joystick.Vertical() / (Time.deltaTime * 1000));
                transform.position += new Vector3(joystick.Horizantal() / (Time.deltaTime * 1000), 0, 0);

                // Move Ball
                ball.transform.position += new Vector3(joystick.Horizantal() / (Time.deltaTime * 500), 0, 0);
                ball.transform.position += new Vector3(0, 0, joystick.Vertical() / (Time.deltaTime * 500));

                // Rotate Ball
                ball.transform.Rotate(Vector3.forward * (joystick.Horizantal() * -500 * Time.deltaTime));
                ball.transform.Rotate(Vector3.right * (joystick.Vertical() * 500 * Time.deltaTime));

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


    }

    private void shoot()
    {

        // Add force to ball
        Rigidbody ballRig = ball.GetComponent<Rigidbody>();

        float angleRad = angle * Mathf.Deg2Rad;

        // Remove all sins from the vectors and pray to gods of physics
        Vector3 targetVector = new Vector3(Mathf.Cos(angleRad * angleMultiplier) * transform.forward.x, Mathf.Sin(angleRad * angleMultiplier), Mathf.Cos(angleRad * angleMultiplier) * transform.forward.z);

        // This multiplier will change the course of the ball if the answer is wrong
        distance *= distanceMultiplier;

        // Speed is calculated using the angle and the distance
        float force = Mathf.Sqrt((distance * (Physics.gravity.y * -1)) / Mathf.Sin(angleRad * 2));


        // Directly set the velocity instead of adding force to have percise control of the ball
        ballRig.velocity = targetVector * force;

        

        Time.timeScale = 1.5f;
        anim.SetBool("isShooting", false);

        // Destroy PlayerControl script from selected Player
        //Destroy(this);
        soundEffectPlayer.PlayKick();
    }
}
