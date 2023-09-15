using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Crane control script

public class Controller_TC : MonoBehaviour
{
    // For correct control, you need to add Rigidbody and ConfigurableJoint
    public Rigidbody boom_point_Rotation;
    public Rigidbody truck;
    public Rigidbody hook;
    public Rigidbody hook_point_Rotation;
    public ConfigurableJoint joint;

    // Assignment of the speed of movement of parts
    public float speed_General = 0.01f; // A common multiplier for changing the speed of all elements
    public float speed_TC_Rails = 4f;
    public float speed_Boom_Rotation = 0.25f;
    public float speed_Truck = 5f;
    public float speed_Hook = 5f;
    public float speed_Hook_Rotation = 0.25f;

    private float distance = 0f; // initial hook position

    void FixedUpdate() 
    {
        // Movement by pressing keys

       // Crane boom rotation
       if (Input.GetKey(KeyCode.RightArrow)) {
            boom_point_Rotation.AddRelativeTorque(0f, speed_Boom_Rotation * speed_General, 0f, ForceMode.VelocityChange);
       }
       if (Input.GetKey(KeyCode.LeftArrow)) {
            boom_point_Rotation.AddRelativeTorque(0f, -speed_Boom_Rotation * speed_General, 0f, ForceMode.VelocityChange);
       }

       // Movement of the track along the boom
       if (Input.GetKey(KeyCode.W)) {
            truck.AddRelativeForce(0f, 0f, speed_Truck * speed_General, ForceMode.VelocityChange);
       }
       if (Input.GetKey(KeyCode.S)) {
            truck.AddRelativeForce(0f, 0f, -speed_Truck * speed_General, ForceMode.VelocityChange);
       }

       // Hook rotation
       if (Input.GetKey(KeyCode.E)) {
            hook_point_Rotation.AddRelativeTorque(0f, speed_Hook_Rotation * speed_General, 0f, ForceMode.VelocityChange);
       }
       if (Input.GetKey(KeyCode.Q)) {
            hook_point_Rotation.AddRelativeTorque(0f, -speed_Hook_Rotation * speed_General, 0f, ForceMode.VelocityChange);
       }

        // Hook height control
        SoftJointLimit limit = joint.linearLimit;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (hook.transform.position.y >= 0.3) 
            { 
                distance += speed_Hook * speed_General; 
            }

        }
        if (Input.GetKey(KeyCode.UpArrow))
        {  
            if (distance >= 0)
            {
                distance -= speed_Hook * speed_General;
            }
        }
        limit.limit = distance;
        joint.linearLimit = limit;

        // To work correctly, you need to wake up Rigidbody
        truck.GetComponent<Rigidbody>().WakeUp();
        hook.GetComponent<Rigidbody>().WakeUp();
    }
}
