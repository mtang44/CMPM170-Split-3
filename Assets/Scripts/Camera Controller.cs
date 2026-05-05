using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 5f;
    public float  RotationSpeed = 5f;
    float cameraVerticalRotation = 0f;
    float inputX;
    float inputY;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Start()
    {
    }
    void Update()
    {
        handlecontrols();
    }


    void handlecontrols()
    {

        inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -60f, 60f);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
    }
}
