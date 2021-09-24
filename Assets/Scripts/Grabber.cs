using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Collider))]
public class Grabber : MonoBehaviour
{
    public Grabber otherHand = null;

    private GameObject collidingObject = null;
    private GameObject grabbedObject = null;
    
    [HideInInspector] public bool grabbing = false;
    [HideInInspector] public bool scaling = false;

    private Vector3 lastHandDifference = Vector3.zero;
    private Quaternion lastAngle = Quaternion.identity;
    private float lastDist = 0.0f;
    private Matrix4x4 lastMidMat = Matrix4x4.identity;
    private Matrix4x4 grabOffset = Matrix4x4.identity;
    private Matrix4x4 lastMatrix = Matrix4x4.identity;

    private float initialDist = 0.0f;
    private Vector3 initialScale = Vector3.one;

    [SerializeField] private bool rightHand = true;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material grabbingMaterial;
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

        bool pickUp = false;
        bool drop = false;
        foreach (var device in thisHandControllers)
        {
            bool triggerValue;
            if (collidingObject != null)
            {
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out triggerValue) && triggerValue)
                {
                    pickUp = true;
                }
                else if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    pickUp = true;
                }
                else
                {
                    drop = true;
                }
            }
        }

        if (pickUp && !(grabbing || scaling))
        {
            PickupObject();
        }
        if (drop)
        {
            DropObject();
        }

        if (collidingObject != null && (grabbing || scaling))
        {
            this.GetComponent<MeshRenderer>().material = grabbingMaterial;
        }
        else if (collidingObject != null)
        {
            this.GetComponent<MeshRenderer>().material = selectedMaterial;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = nonSelectedMaterial;
        }

        Matrix4x4 deltaMatrix;

        Vector3 currentHandDifference = transform.position - otherHand.transform.position;
        float currentDist = Vector3.Magnitude(currentHandDifference);

        float distDelta = currentDist - lastDist;
        Quaternion rotDelta = Quaternion.FromToRotation(lastHandDifference, currentHandDifference);

        lastDist = currentDist;
        lastHandDifference = currentHandDifference;

        if (grabbing && grabbedObject != null && !otherHand.scaling)
        {
            Matrix4x4 currentMatrix = this.transform.localToWorldMatrix;
            deltaMatrix =  currentMatrix * lastMatrix.inverse;
            lastMatrix = currentMatrix;

            Matrix4x4 newObjMat = deltaMatrix * grabbedObject.transform.localToWorldMatrix;
            grabbedObject.transform.FromMatrix(newObjMat);   
        }

        if (scaling && otherHand.grabbedObject != null)
        {
            float pDist = currentDist / initialDist;

            Vector3 averagePos = (this.transform.position + otherHand.transform.position)/2;
            Vector3 between = otherHand.transform.position - this.transform.position;
            Vector3 averageUp = (this.transform.up + otherHand.transform.up) / 2;

            Vector3 averageForward = Vector3.Cross(between, averageUp.normalized);
            averageUp = -Vector3.Cross(between, averageForward.normalized);

            Quaternion averageRot = Quaternion.LookRotation(averageForward.normalized,averageUp);
            float sharedScale = initialScale.x * pDist;

            Matrix4x4 currentMidMat = Matrix4x4.TRS(averagePos, averageRot, sharedScale*Vector3.one);
            deltaMatrix = currentMidMat * lastMidMat.inverse;

            lastMidMat = currentMidMat;
            Matrix4x4 newObjMat = deltaMatrix * otherHand.grabbedObject.transform.localToWorldMatrix;
            otherHand.grabbedObject.transform.FromMatrix(newObjMat);
        }

        lastMatrix = this.transform.localToWorldMatrix;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Grabbable>() != null)
        {
            collidingObject = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        DropObject();
        collidingObject = null;
    }

    void PickupObject()
    {
        if (collidingObject != null)
        {
            if (!otherHand.grabbing)
            {
                grabbing = true;
                grabbedObject = collidingObject;
            }
            else
            {
                scaling = true;
                lastDist = Vector3.Distance(transform.position, otherHand.transform.position);
                initialDist = lastDist;
                initialScale = collidingObject.transform.localScale;
                lastAngle = collidingObject.transform.rotation;

                float pDist = initialDist / initialDist;

                Vector3 averagePos = (this.transform.position + otherHand.transform.position) / 2;
                Vector3 between = otherHand.transform.position - this.transform.position;
                Vector3 averageUp = (this.transform.up + otherHand.transform.up) / 2;

                Vector3 averageForward = Vector3.Cross(between, averageUp);
                averageUp = -Vector3.Cross(between, averageForward);

                Quaternion averageRot = Quaternion.LookRotation(averageForward, averageUp);
                float sharedScale = initialScale.x * pDist;

                lastMidMat = Matrix4x4.TRS(averagePos, averageRot, sharedScale * Vector3.one);
            }
        }
    }

    void DropObject()
    {
        grabbedObject = null;
        grabbing = false;
        scaling = false;
        if (otherHand.scaling)
        {
            otherHand.PickupObject();
        }
    }
}
