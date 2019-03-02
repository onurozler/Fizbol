using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.25f;
    public float decreaseFactor = 1.0f;

    private float timeLeft = 3.5f;

    public GameObject finishTheGame;

    Vector3 originalPos;

    private void Start()
    {
        finishTheGame= GameObject.Find("LevelManager").GetComponent<LevelManager>().repeatGame;
    }

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
        }
        else
        {
            finishTheGame.SetActive(true);
        }
    }
}