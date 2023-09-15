using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这段代码是一个货物与夹子之间的交互脚本。当夹子进入触发器时，触发器的回调函数会将contactHook变量设置为true，并禁用触发器和启用夹子的SkinnedMeshRenderer组件。在FixedUpdate函数中，如果contactHook为true，则将货物的位置和旋转设置为夹子的位置和旋转，并禁用夹子的BoxCollider组件的isTrigger属性。如果按下空格键，则将contactHook变量设置为false，并启用触发器和禁用夹子的SkinnedMeshRenderer组件。同时，启用触发器的Rigidbody组件的isKinematic属性，禁用夹子的BoxCollider组件的isTrigger属性。
// Hook勾中Cargo时，鉴定塔吊连接到物体时的状态，以及被TargetCargo.cs调用时操作Cargo到达目标位置的行为
public class CargoContact : MonoBehaviour
{
    public GameObject Point_Rotation_Hook;
    public GameObject clamps;
    public GameObject trigger_ancoragePoint;

    public bool contactHook = false;

    // 当钩子到达目标位置trigger_ancoragePoint,修改contactHook状态为true,隐藏该点，并放出吊线拉住物体Cargo
    private void OnTriggerEnter(Collider other)
    {
        contactHook = true;
        trigger_ancoragePoint.GetComponent<MeshRenderer>().enabled = false;
        clamps.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    void FixedUpdate()
    {
        // Joining and following the hook
        if (contactHook == true)
        {
            transform.position = Point_Rotation_Hook.transform.position;
            transform.rotation = Point_Rotation_Hook.transform.rotation;
            Point_Rotation_Hook.GetComponent<BoxCollider>().isTrigger = false;
        }

        if (contactHook == false)
        {
            Point_Rotation_Hook.GetComponent<BoxCollider>().isTrigger = true;
        }

        // Cargo disconnection
        if (Input.GetKey(KeyCode.Space))
        {
            contactHook = false;
            // 受物理引擎影响，自动掉落并与接触面产生碰撞
            trigger_ancoragePoint.GetComponent<Rigidbody>().isKinematic = false;
            clamps.GetComponent<SkinnedMeshRenderer>().enabled = false;
            Point_Rotation_Hook.GetComponent<BoxCollider>().isTrigger = true;
        }
    }
}
