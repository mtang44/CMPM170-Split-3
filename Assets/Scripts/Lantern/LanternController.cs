using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LanternFollowSmooth : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;

    [Header("Forward Placement")]
    public float forwardDistance = 6f;
    public float rightOffset = -3.5f;
    public float upOffset = 1f;

    [Header("Follow Settings")]
    public float baseSmoothTime = 0.12f;
    public float maxSpeedForLagReduction = 6f;

    [Header("Movement Swing")]
    public float rotationAmount = 12f;
    public float swayMultiplier = 2f;

    [Header("Player Rotation Sway")]
    public float rotationSwayAmount = 4f;
    public float rotationSwaySmoothness = 8f;

    [Header("Rotation Smooth")]
    public float rotationSmoothSpeed = 8f;

    [Header("Idle Sway")]
    public float idleSwayAmount = 2f;
    public float idleSwaySpeed = 1.5f;

    [Header("Sway Sound")]
    public AudioClip lowSwayClip; 
    public AudioClip highSwayClip; 

    public float lowThreshold = 0.15f;
    public float highThreshold = 100f;

    public float swaySoundCooldown = 0.08f;

    [Header("Pitch Variance")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Volume Variance")]
    public float minVolume = 0.85f;
    public float maxVolume = 1f;

    private Vector3 velocity;
    private Vector3 lastPlayerPos;

    private Quaternion currentRotation;

    private float lastPlayerYRotation;
    private Vector3 rotationSway;

    private AudioSource audioSource;
    private float nextSoundTime;

    void Start()
    {
        if (!player || !cameraTransform) return;

        lastPlayerPos = player.position;
        currentRotation = transform.rotation;

        lastPlayerYRotation = player.eulerAngles.y;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void LateUpdate()
    {
        if (!player || !cameraTransform)
        {
            Debug.LogWarning("assign the player and camera transform in inspector");
            return;
        }

        Vector3 moveDelta = player.position - lastPlayerPos;

        float speed =
            moveDelta.magnitude /
            Mathf.Max(Time.deltaTime, 0.0001f);

        float speed01 = Mathf.Clamp01(speed / maxSpeedForLagReduction);

        float smoothTime = Mathf.Lerp(baseSmoothTime, 0.02f, speed01);

        Vector3 targetPos =
            cameraTransform.position +
            cameraTransform.forward * forwardDistance +
            cameraTransform.right * rightOffset +
            cameraTransform.up * upOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        Vector3 localMove =
            player.InverseTransformDirection(moveDelta);

        float moveTiltX =
            localMove.z * rotationAmount * swayMultiplier;

        float moveTiltZ =
            -localMove.x * rotationAmount * swayMultiplier;

        float currentYRotation = player.eulerAngles.y;

        float rotationDelta = Mathf.DeltaAngle(
            lastPlayerYRotation,
            currentYRotation
        );

        Vector3 targetRotationSway = new Vector3(
            0f,
            0f,
            -rotationDelta * rotationSwayAmount
        );

        rotationSway = Vector3.Lerp(
            rotationSway,
            targetRotationSway,
            Time.deltaTime * rotationSwaySmoothness
        );

        float idleTilt =
            Mathf.Sin(Time.time * idleSwaySpeed) * idleSwayAmount;

        Quaternion targetRotation =
            cameraTransform.rotation *
            Quaternion.Euler(
                moveTiltX + idleTilt,
                0f,
                moveTiltZ + rotationSway.z
            );

        currentRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            Time.deltaTime * rotationSmoothSpeed
        );

        transform.rotation = currentRotation;

        //sound stuff

        float swayAmount =
            Mathf.Abs(moveTiltX) +
            Mathf.Abs(moveTiltZ) +
            Mathf.Abs(rotationSway.z);

        if (Time.time > nextSoundTime)
        {
            // HIGH IMPACT SOUND
            if (swayAmount >= highThreshold && highSwayClip != null)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.volume = Random.Range(minVolume, maxVolume);

                audioSource.PlayOneShot(highSwayClip);
                nextSoundTime = Time.time + swaySoundCooldown;
            }
            // LOW/MID SWAY SOUND
            else if (swayAmount > lowThreshold && lowSwayClip != null)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.volume = Random.Range(minVolume, maxVolume);

                audioSource.PlayOneShot(lowSwayClip);
                nextSoundTime = Time.time + swaySoundCooldown;
            }
        }

        lastPlayerPos = player.position;
        lastPlayerYRotation = currentYRotation;
    }
}