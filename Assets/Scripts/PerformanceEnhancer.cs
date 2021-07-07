using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using IVLab.ABREngine;
using Newtonsoft.Json.Linq;

public class PerformanceEnhancer : MonoBehaviour
{
    // Render every 10 seconds (600 frames)
    private int slowInterval = 600;

    // Quick interval for rendering (every 1 frame)
    private int quickInterval = 1;

    // Frames since speedy render called
    private int framesSinceSpeedyRender = 0;

    // Frames to wait before slowing down again
    private int framesToBeFast = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Assign callback for when ABREngine receives new state
        ABREngine.Instance.OnStateChanged += StateUpdated;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        OnDemandRendering.renderFrameInterval = slowInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (framesSinceSpeedyRender > framesToBeFast)
        {
            OnDemandRendering.renderFrameInterval = slowInterval;
        }
        else
        {
            framesSinceSpeedyRender += 1;
        }

        if (Input.anyKey)
        {
            SpeedyRender();
        }
    }

    void StateUpdated(JObject _state)
    {
        SpeedyRender();
    }

    void SpeedyRender()
    {
        OnDemandRendering.renderFrameInterval = quickInterval;
        framesSinceSpeedyRender = 0;
    }
}