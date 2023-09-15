using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 定义要运送的物体的行为，在环境中出发OnTriggerEnter函数时，即完成任务，进行环境操作
public class Cargo : MonoBehaviour
{
    public GameObject clamps;
    public GameObject target;
    public GameObject cargo;

    public bool contactTarget = false;

    // When the hook hits the trigger it disappears and clamps appear
    private void OnTriggerEnter(Collider other)
    {
        contactTarget = true;

        //触发contact后，即触及目标位置，使cargo自动定位到该位置位置
        cargo.transform.position = target.transform.position;
        cargo.transform.rotation = target.transform.rotation;
        
        // 隐藏夹子的外观
        clamps.GetComponent<SkinnedMeshRenderer>().enabled = false;
        
        // 使其在夹子的控制下移动，而不受重力的影响。
        cargo.GetComponent<Rigidbody>().useGravity = false;
        
        // isKinematic是Rigidbody组件的一个属性，用于控制物体是否受到物理引擎的影响。
        // 当isKinematic属性为true时，物体不会受到物理引擎的影响，可以通过代码来控制其运动；
        // 当isKinematic属性为false时，物体会受到物理引擎的影响，会受到重力、碰撞等力的作用。
        // 通常情况下，如果需要通过代码来控制物体的运动，可以将isKinematic属性设置为true。
        cargo.GetComponent<Rigidbody>().isKinematic = true;
        
        // 隐藏目标的外观
        target.GetComponent<MeshRenderer>().enabled = false;
        // isTrigger属性设置为false可以使目标产生物理碰撞
        target.GetComponent<BoxCollider>().isTrigger = false;
        // 禁用其碰撞检测
        target.GetComponent<BoxCollider>().enabled = false;
    }
}
