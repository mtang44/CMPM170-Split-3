using UnityEngine;

public class Final : MonoBehaviour
{
    public Light finalLight;
    public GameObject door;

    public void ActivateFinalObject()
    {

        if (finalLight != null)
        {
            Debug.Log("Set final Light on");
            finalLight.enabled = true;
        }

        if (door != null)
        {
            Debug.Log("Gate open");
            door.SetActive(false);
        }
    }
}