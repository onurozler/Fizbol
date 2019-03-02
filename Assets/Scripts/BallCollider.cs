using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallCollider : MonoBehaviour
{
    public bool isForvet = false;

    private LevelManager levelManager;
    private SoundEffectsController soundEffectPlayer;
    private void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        soundEffectPlayer = GameObject.Find("SoundEffectController").GetComponent<SoundEffectsController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "goalTarget")
        {
            Time.timeScale = 1f;

            soundEffectPlayer.PlayGoal();

            // Goal
            Destroy(levelManager.mainCamera.GetComponent<CameraController>());

            levelManager.mainCamera.gameObject.AddComponent<CameraShake>();

            levelManager.questionManager.trueOrFalse.GetComponent<Text>().color = Color.white;
            levelManager.questionManager.trueOrFalse.GetComponent<Text>().text = "Goool!!";
            levelManager. questionManager.trueOrFalse.GetComponent<Animator>().SetTrigger("setAnim");

            levelManager.allPlayers.transform.GetChild(0).GetChild(5).GetComponent<Animator>().SetTrigger("isGoal");
            Destroy(levelManager.allPlayers.transform.GetChild(1).GetChild(0).GetComponent<Animator>());

            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        Time.timeScale = 1f;
        if (isForvet)
        {
            if(collision.transform.tag == "red")
            {
                // Goalkeeper saves.
                levelManager.restartLastPosition();
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            if (collision.transform.tag == "target")
            {
                Debug.Log("SUCCESS");
                levelManager.playerGotTheBall();
            }
            else
            {
                levelManager.restartLastPosition();
            }
        }
    }
}
