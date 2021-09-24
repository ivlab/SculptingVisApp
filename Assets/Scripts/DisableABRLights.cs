using UnityEngine;
using IVLab.ABREngine;

[RequireComponent(typeof(ABREngine))]
public class DisableABRLights : MonoBehaviour
{
    void Update()
    {
        foreach (var light in GetComponentsInChildren<Light>())
        {
            light.enabled = false;
        }
    }
}