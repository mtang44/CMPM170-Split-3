using UnityEngine;

public class BridgeRotator : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    public Transform bridgePivot;

    public Vector3 raisedRotation = new Vector3(0f, 0f, 0f);
    public Vector3 loweredRotation = new Vector3(0f, 0f, 0f);

    public float rotateSpeed = 60f;

    private bool playerNearby = false;
    private bool activated = false;

    void Start()
    {
        if (bridgePivot != null)
        {
            bridgePivot.localEulerAngles = raisedRotation;
        }
    }

    void Update()
    {
        if (playerNearby && !activated && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Lever activated, bridge lowering");
            activated = true;
        }

        if (activated && bridgePivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(loweredRotation);

            bridgePivot.localRotation = Quaternion.RotateTowards(
                bridgePivot.localRotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player near lever");
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left lever");
            playerNearby = false;
        }
    }
}
