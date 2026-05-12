using UnityEngine;

public class StickHandSway : MonoBehaviour
{
    [Header("Locked Local Transform")]
    public Vector3 baseLocalPosition = new Vector3(-1.02f, 0.27f, 1.3f);
    public Vector3 baseLocalEulerRotation = new Vector3(41.42f, -12f, 0f);

    [Header("Sway Settings")]
    public float yawStrength = 5f;     // strong influence on horizontal movement
    public float pitchStrength = 1.5f; // lighter influence
    public float rollStrength = 0.3f;  // almost nothing
    public float smooth = 10f;

    private Quaternion baseRotation;
    private Quaternion targetRotation;

    void Start()
    {
        transform.localPosition = baseLocalPosition;
        baseRotation = Quaternion.Euler(baseLocalEulerRotation);
        transform.localRotation = baseRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // favor Y movement (turning left/right)
        float yaw = mouseX * yawStrength;

        // Light vertical influence
        float pitch = -mouseY * pitchStrength;

        // minimal roll for subtle realism
        float roll = -mouseX * rollStrength;

        Quaternion sway = Quaternion.Euler(pitch, yaw, roll); //thank you random reddit user
        targetRotation = baseRotation * sway;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smooth
        );

        transform.localPosition = baseLocalPosition;
    }
}