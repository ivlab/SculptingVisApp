using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using IVLab.ABREngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// Performance enhancement for older computers running ABR. Essentially, one
/// frame is rendered every 10 seconds *unless* the ABR state is changed OR
/// there is mouse/keyboard input.
/// </summary>
public class PerformanceEnhancer : MonoBehaviour
{
    // Render every 10 seconds (600 frames)
    [SerializeField] private int slowFrameInterval = 600;

    // Quick interval for rendering (every 1 frame)
    [SerializeField] private int fastFrameInterval = 1;

    // Target frame rate for the app
    [SerializeField] private int targetFrameRate = 60;

    // Frames since speedy render called
    private int framesSinceSpeedup = 0;

    // Frames to wait before slowing down again
    private int framesToBeFast = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Assign callback for when ABREngine receives new state
        ABREngine.Instance.OnStateChanged += StateUpdated;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        OnDemandRendering.renderFrameInterval = slowFrameInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (framesSinceSpeedup > framesToBeFast)
        {
            OnDemandRendering.renderFrameInterval = slowFrameInterval;
        }
        else
        {
            framesSinceSpeedup += 1;
        }

        if (Input.anyKey)
        {
            SpeedUp();
        }
    }

    void StateUpdated(JObject _state)
    {
        SpeedUp();
    }

    void SpeedUp()
    {
        OnDemandRendering.renderFrameInterval = fastFrameInterval;
        framesSinceSpeedup = 0;
    }
}