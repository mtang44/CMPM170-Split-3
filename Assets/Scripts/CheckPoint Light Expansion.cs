using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CheckPointLightExpansion : MonoBehaviour
{
    public GameObject LightSphere;
    public int maxSphereSize;
    public Vector3 growRate;
    public GameObject DirectionalLight;
    private bool startGrowing = false;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        if(startGrowing)
        {
            if(LightSphere.transform.localScale.y < maxSphereSize)
            {
                Debug.Log("Growing");
                LightSphere.transform.localScale += growRate; 
            }
            else
            {
                LightSphere.SetActive(false);

                Debug.Log("Max Size Reached");
            }
           
        }
    }
    public void BeginSphereGrow()
    {
        Debug.Log("begin growing");
       startGrowing = true;
    }
    void OnTriggerEnter(Collider Other)
    {
        if(Other.gameObject.tag == "Player")
        {
            DirectionalLight.SetActive(true);
        }
    }

}
