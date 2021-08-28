using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Collider))]
public class Grabber : MonoBehaviour
{
    private Transform oldParent = null;
    private GameObject collidingObject = null;

    [SerializeField] private bool rightHand = true;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material nonSelectedMaterial;

    // Update is called once per frame
    void Update()
    {
        InputDeviceCharacteristics handChar = InputDeviceCharacteristics.None;
        if (rightHand)
        {
            handChar = InputDeviceCharacteristics.Right;
        }
        else
        {
            handChar = InputDeviceCharacteristics.Left;
        }
        var thisHandControllers = new List<InputDevice>();
        var desiredCharacteristics =
            InputDeviceCharacteristics.HeldInHand
            | handChar
            | InputDeviceCharacteristics.Controller
            | InputDeviceCharacteristics.TrackedDevice;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, thisHandControllers);

        foreach (var device in thisHandControllers)
        {
            bool triggerValue;
            if (collidingObject != null)
            {
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out triggerValue) && triggerValue)
                {
                    collidingObject.transform.parent = transform;
                }
                else
                {
                    collidingObject.transform.parent = oldParent;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Grabbable>() != null)
        {
            collidingObject = other.gameObject;
            oldParent = collidingObject.transform.parent;
            this.GetComponent<MeshRenderer>().material = selectedMaterial;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (collidingObject != null)
        {
            collidingObject.transform.parent = oldParent;
        }
        oldParent = null;
        collidingObject = null;
        this.GetComponent<MeshRenderer>().material = nonSelectedMaterial;
    }
}
