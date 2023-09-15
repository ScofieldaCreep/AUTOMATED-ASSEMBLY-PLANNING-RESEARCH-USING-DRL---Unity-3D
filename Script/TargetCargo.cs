using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 当货物进入目标区域时，会调用CargoContact脚本中的回调函数来实现货物的卸载

public class TargetCargo : MonoBehaviour
{
    // Checking the intersection with the cargo trigger
    private void OnTriggerEnter(Collider col)
    {
        col.GetComponent<CargoContact>();
    }
}
