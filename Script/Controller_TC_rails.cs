using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于控制起重机的脚本。脚本中定义了起重机的各个部件的Rigidbody和ConfigurableJoint组件，并通过按键来控制各个部件的运动。其中，通过AddRelativeForce和AddRelativeTorque方法来控制物体的移动和旋转，通过linearLimit属性来控制钩子的高度。同时，为了保证物体的运动正确，需要使用WakeUp方法来唤醒Rigidbody组件。

public class Controller_TC_rails : MonoBehaviour
{
    // For correct control, you need to add Rigidbody and ConfigurableJoint
    public Rigidbody tC_Rails;
    public Rigidbody boom_point_Rotation;
    public Rigidbody truck;
    public Rigidbody hook;
    public Rigidbody hook_point_Rotation;
    public ConfigurableJoint joint;

    // Assignment of the speed of movement of parts
    public float speed_General = 0.01f; // A common multiplier for changing the speed of all elements
    public float speed_TC_Rails = 10f;
    public float speed_Boom_Rotation = 1f;
    public float speed_Truck = 15f;
    public float speed_Hook = 0.05f;
    public float speed_Hook_Rotation = 0.25f;

    private float distance = 0f; // initial hook position

    // Initial positions and rotations
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initialBoomRotation;
    private Vector3 initialTruckPosition;
    private Vector3 initialHookPosition;
    private Quaternion initialHookRotation;

    public SoftJointLimit limit;

     // 在Unity中，Awake()方法会在脚本被加载时自动调用，用于初始化变量和获取初始位置、旋转等信息。
    private void Awake()
    {
        // Initialize initial positions and rotations
        initialPosition = tC_Rails.transform.position;
        initialRotation = tC_Rails.transform.rotation;
        initialBoomRotation = boom_point_Rotation.transform.rotation;
        initialTruckPosition = truck.transform.position;
        initialHookPosition = hook.transform.position;
        initialHookRotation = hook_point_Rotation.transform.rotation;
    }

     public void Reset()
    {
        // Reset positions and rotations to initial values
        tC_Rails.transform.position = initialPosition;
        tC_Rails.transform.rotation = initialRotation;
        boom_point_Rotation.transform.rotation = initialBoomRotation;
        truck.transform.position = initialTruckPosition;
        hook.transform.position = initialHookPosition;
        hook_point_Rotation.transform.rotation = initialHookRotation;

        // Reset hook joint distance
        SoftJointLimit limit = joint.linearLimit;
        limit.limit = 0f;
        joint.linearLimit = limit;

        // Wake up Rigidbody components
          CollideWakeUp();
    }


     // FixedUpdate也是Unity引擎中的一个回调函数，用于在每一帧更新时执行相应的操作。与Update函数不同的是，FixedUpdate函数的调用频率是固定的，不受帧率的影响。在物理模拟中，通常使用FixedUpdate函数来更新物体的位置和旋转等属性，以保证物理模拟的正确性。开发者可以在脚本中重写该函数，并在其中编写自己的逻辑代码。当每一帧更新时，Unity引擎会自动调用该函数来执行相应的操作。
    void FixedUpdate() 
    {
        // Movement by pressing keys

     // Moving the crane along the rails
     // ForceMode.VelocityChange使物体受到的力直接改变其速度，而不受物体质量的影响，模拟匀速运动
       if (Input.GetKey(KeyCode.D)) {
            MoveRail(1);
       }
       if (Input.GetKey(KeyCode.A)) {
            MoveRail(2);
       }
       if (Input.GetKey(KeyCode.Space)) {
          truck.velocity = Vector3.zero;
       }

       // Crane boom rotation 水平吊臂，产生绕y轴旋转的力，改变其速度
       if (Input.GetKey(KeyCode.RightArrow)) {
          RotateBoom(1);
       }
       if (Input.GetKey(KeyCode.LeftArrow)) {
          RotateBoom(2);
       }

       // Movement of the track along the boom
       // 由于使用了AddRelativeForce方法，所以施加的力是相对于物体自身坐标系的，而不是世界坐标系。因此，施加的力是沿着物体自身坐标系的z轴方向，即沿着crane boom方向。
       if (Input.GetKey(KeyCode.W)) {
          MoveTruck(1);    
       }
       if (Input.GetKey(KeyCode.S)) {
          MoveTruck(2);
       }

       // Hook rotation
       if (Input.GetKey(KeyCode.E)) {
          RotateHook(1);
       }
       if (Input.GetKey(KeyCode.Q)) {
          RotateHook(2);
       }

        // Hook height control
        // ConfigurableJoint是Unity中的一个组件，用于控制物体之间的连接和运动。linearLimit属性用于控制连接物体之间的线性限制，此处使用limit变量来控制钩子的高度。
        
        if (Input.GetKey(KeyCode.DownArrow))
        {
            SetHookHeight(1);

        }
        if (Input.GetKey(KeyCode.UpArrow))
        {  
            SetHookHeight(2);
        }

        // To work correctly, you need to wake up Rigidbody
        // 物体的Rigidbody组件在运动过程中，如果速度或角速度较小，就可能会进入睡眠状态。睡眠状态是指物体的Rigidbody组件暂停计算，以节省计算资源。当物体受到外力作用时，Rigidbody组件会自动唤醒，重新开始计算。在某些情况下，如果物体的Rigidbody组件长时间处于睡眠状态，可能会影响物体的运动正确性，需要使用WakeUp方法来唤醒它们。
        truck.GetComponent<Rigidbody>().WakeUp();
        hook.GetComponent<Rigidbody>().WakeUp();
        
    }

     public void MoveRail(int direction) {
          if (direction == 1) {
               tC_Rails.AddRelativeForce(0f, 0f, -speed_TC_Rails * speed_General, ForceMode.VelocityChange);
          }
          else if (direction == 2) {
               tC_Rails.AddRelativeForce(0f, 0f, speed_TC_Rails * speed_General, ForceMode.VelocityChange);
          }
          else {tC_Rails.velocity = Vector3.zero;}
          CollideWakeUp();
     }

     public void RotateBoom(int direction) {
     // 根据传入的参数实现起重机吊臂旋转的逻辑
     // 在这里编写旋转起重机吊臂的代码
          if (direction == 1) {
            boom_point_Rotation.AddRelativeTorque(0f, speed_Boom_Rotation * speed_General, 0f, ForceMode.VelocityChange);
          }
          else if (direction == 2) {
            boom_point_Rotation.AddRelativeTorque(0f, -speed_Boom_Rotation * speed_General, 0f, ForceMode.VelocityChange);
          }
          else {boom_point_Rotation.angularVelocity = Vector3.zero;}
          CollideWakeUp();
     }

     public void MoveTruck(int direction) // bug
     {
     // 根据传入的参数实现起重机的平移移动的逻辑
     // 在这里编写平移起重机的代码
          if (direction == 1) {
               truck.AddRelativeForce(0f, 0f, speed_Truck * speed_General, ForceMode.VelocityChange);
          }
          else if (direction == 2) {
               truck.AddRelativeForce(0f, 0f, -speed_Truck * speed_General, ForceMode.VelocityChange);
          }
          else {truck.velocity = Vector3.zero;}
          CollideWakeUp();
     }

     public void RotateHook(int direction) // bug
     {
          if (direction == 1) {
               hook_point_Rotation.AddRelativeTorque(0f, speed_Hook_Rotation * speed_General * 10, 0f, ForceMode.VelocityChange);
          }
          else if (direction == 2) {
               hook_point_Rotation.AddRelativeTorque(0f, -speed_Hook_Rotation * speed_General * 10, 0f, ForceMode.VelocityChange);
          }
          else {hook_point_Rotation.angularVelocity = Vector3.zero;}
          CollideWakeUp();
     }

     public void SetHookHeight(int direction)
     {
        SoftJointLimit limit = joint.linearLimit;
        {
        if (direction == 1)
            if (hook.transform.position.y >= 0.3) 
            { 
                distance += speed_Hook * speed_General; 
            }

        }
        if (direction == 2)
        {  
            if (distance >= 0)
            {
                distance -= speed_Hook * speed_General;
            }
        }
        limit.limit = distance;
        joint.linearLimit = limit;
     }

     public void CollideWakeUp() {

          // 定义一个标签，用于标识环境中的物体
          string collideTag = "Collide";
          string collideTag2  = "correct_collide";
          // 获取带有指定标签的所有物体
          GameObject[] collideObjects = GameObject.FindGameObjectsWithTag(collideTag);

          // 遍历物体数组，并唤醒刚体组件
          foreach (GameObject obj in collideObjects)
          {
               // 获取物体上的刚体组件
               Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

               // 确保物体上有刚体组件
               if (rigidbody != null)
               {
                    // 唤醒刚体组件
                    rigidbody.WakeUp();
               }
          }

          collideObjects = GameObject.FindGameObjectsWithTag(collideTag2);

          foreach (GameObject obj in collideObjects)
          {
               Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

               if (rigidbody != null)
               {
                    rigidbody.WakeUp();
               }

          }
     }
}