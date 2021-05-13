using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public CUIColorPicker picker;
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        picker.Color = mainCam.backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {
        mainCam.backgroundColor = picker.Color;
    }
}
