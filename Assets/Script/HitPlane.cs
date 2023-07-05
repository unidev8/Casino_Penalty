using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class HitPlane : MonoBehaviour
{
    //[SerializeField]
    //private GameObject hitMark;
    private GameObject instHitMark;    
    private const float contDisapeareTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
      
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.tag == "Ball")//
        {
            Debug.Log("HitPlane Hit! Point of contact: " + other.gameObject.transform.position );
            instHitMark = GameObject.Instantiate(hitMark, new Vector3(other.gameObject.transform.position.x, gameObject.transform.position.y, other.gameObject.transform.position.z - 0.2f), hitMark.transform.rotation);
            GameManager.Destroy(instHitMark, contDisapeareTime);
            other.gameObject.tag = "Untagged";
            this.gameObject.GetComponent<Renderer>().enabled = true;
         }
        */
    }
    // Update is called once per frame
    void Update()
    {
        //if (instHitMark)
        {
            //instHitMark.transform.RotateAround(instHitMark.transform.position, Vector3.forward, Time.deltaTime * 10f); 
        }
        
    }
}
