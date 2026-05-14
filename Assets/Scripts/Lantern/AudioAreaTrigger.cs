using UnityEngine;

public class AudioAreaTrigger : MonoBehaviour
{
    public AudioManager.AreaType areaType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager audioManager = FindObjectOfType<AudioManager>();

            if (audioManager != null)
            {
                audioManager.ChangeArea(areaType);
            }
        }
    }
}