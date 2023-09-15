using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public bool contactHook = false;
    // Checking the intersection with the cargo trigger
    private void OnTriggerEnter(Collider col)
    {
        col.GetComponent<CargoContact>();
        contactHook = true;
    }
}
