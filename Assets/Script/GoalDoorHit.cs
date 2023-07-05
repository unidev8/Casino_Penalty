using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class GoalDoorHit : MonoBehaviour
{  
    private float forceMultiplier = -40f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("GoalDoor Hit! gameObject = " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Ball")
        {
            //ContactPoint contact = collision.contacts[0];            
            collision.gameObject.tag = "Untagged";       // protect recollision of ball;
            if (GameManager.instance.goalState == 0)
            {
                Vector3 ballForce = PlayerActions.instance.BallDir * forceMultiplier;
                GameManager.instance.goalNet_1.randomAcceleration = new Vector3(ballForce.x, ballForce.y, ballForce.z);
                GameManager.instance.goalNet_2.randomAcceleration = new Vector3(ballForce.x, ballForce.y, ballForce.z);
                GameManager.instance.goalNet_3.randomAcceleration = new Vector3(ballForce.x, ballForce.y, ballForce.z);
                GameManager.instance.goalNet_4.randomAcceleration = new Vector3(ballForce.x, ballForce.y, ballForce.z);
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
