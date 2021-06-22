using System;
using UnityEngine;
using IVLab.ABREngine;
using IVLab.Utilities;
using Newtonsoft.Json.Linq;


public class BackgroundColor : MonoBehaviour
{
    public CUIColorPicker picker;
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        picker.Color = mainCam.backgroundColor;
        ABREngine.Instance.OnStateChanged += OnABRStateChanged;
    }

    void OnABRStateChanged(JObject state)
    {
        try
        {
            string bgColorHtml = state["scene"]?["backgroundColor"]?.ToString();
            Color bgColor = picker.Color;
            if (bgColorHtml != null)
            {
                bgColor = IVLab.Utilities.ColorUtilities.HexToColor(bgColorHtml);
            }
            else
            {
                Debug.LogErrorFormat("Unable to parse color: {0}", bgColorHtml);
            }
            picker.Color = bgColor;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SaveBackground()
    {
        ABREngine.Instance.SaveStateAsync();
    }

    // Update is called once per frame
    void Update()
    {
        mainCam.backgroundColor = picker.Color;
    }
}
