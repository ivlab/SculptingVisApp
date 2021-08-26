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

    // Time since speedy render called
    private float timeSinceSpeedup = 0;

    // Time to wait before slowing down again
    private float timeToBeFast = 10.0f / 60.0f;  // 10 frames at 60 fps

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
        if (timeSinceSpeedup > timeToBeFast)
        {
            OnDemandRendering.renderFrameInterval = slowFrameInterval;
        }
        else
        {
            timeSinceSpeedup += Time.deltaTime;
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
        // Only ever increase the length of time that rendering is sped up for
        if (timeSinceSpeedup > 0) timeSinceSpeedup = 0;
    }

    public void SpeedUpForSeconds(float speedUpTime)
    {
        OnDemandRendering.renderFrameInterval = fastFrameInterval;
        // Only ever increase the length of time that rendering is sped up for
        if (timeToBeFast - timeSinceSpeedup < speedUpTime) timeSinceSpeedup = timeToBeFast - speedUpTime;
    }
}
