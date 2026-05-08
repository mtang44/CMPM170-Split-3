using UnityEngine;

public class LightTrail : MonoBehaviour
{
    public Transform player;
    public GameObject lightStampPrefab;

    public float distanceBetweenStamps = 3f;
    public bool withinAnotherLight = false;
    public GameObject LightTrailHolder;

    private Vector3 lastStampPos;

    void Start()
    {
        lastStampPos = player.position;
    }

    void Update()
    {
        if (Vector3.Distance(player.position, lastStampPos) > distanceBetweenStamps && !withinAnotherLight)
        {
            SpawnStamp();

            lastStampPos = player.position;
        }
    }

    void SpawnStamp()
    {
        Vector3 pos = new Vector3(player.position.x,player.position.y + 5, player.position.z);
        GameObject newLight = Instantiate(lightStampPrefab, pos, Quaternion.Euler(90, Random.Range(0, 360), 0));
        newLight.transform.SetParent(LightTrailHolder.transform);
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "spotlight trail")
        {
            if(!withinAnotherLight)
            {
                withinAnotherLight = true;
            }
         
        }
    }
    void OnTriggerExit(Collider other)
    {
         if(other.gameObject.tag == "spotlight trail")
        {
            if(withinAnotherLight)
            {
                withinAnotherLight = false;
            }
        }
    }
}