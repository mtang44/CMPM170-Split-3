using UnityEngine;

public class InteractableTarget : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    public Light topLight;
    public Light fullLight;

    public PuzzleManager manager;

    private bool playerNearby = false;
    private bool activated = false;

    void Start()
    {

        if (topLight != null)
        {
            topLight.enabled = true;
        }

        if (fullLight != null)
        {
            fullLight.enabled = false;
        }
    }

    void Update()
    {
        if (playerNearby && !activated && Input.GetKeyDown(interactKey))
        {
            Debug.Log("E triggered");
            Activate();
        }
    }

    void Activate()
    {
        Debug.Log("In activate");
        activated = true;

        if (fullLight != null)
        {
            fullLight.enabled = true;
            Debug.Log("Lightup");
        }

        if (manager != null)
        {
            manager.ActivateOneObject();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("set nearby true");
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("set nearby false");
            playerNearby = false;
        }
    }
}
