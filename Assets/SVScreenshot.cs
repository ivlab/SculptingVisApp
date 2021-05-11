using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVLab.Utilities;

public class SVScreenshot : MonoBehaviour
{
    public InputField pathText;
    public InputField nameText;
    public InputField widthText;
    public InputField heightText;
    public Toggle transBg;
    public Screenshot screenshotCamera;

    void Start()
    {
        var homevar = "";
        if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
            homevar = "HOMEPATH";
        else
            homevar = "HOME";
        var home = System.Environment.GetEnvironmentVariable(homevar);
        string defaultScreenshotPath = System.IO.Path.Combine(System.IO.Path.Combine(home, "Desktop"));
        string defaultNameText = "capture.png";

        nameText.text = defaultNameText;
        pathText.text = defaultScreenshotPath;
        widthText.text = "1920";
        heightText.text = "1080";
    }

    public void SaveScreenshot() {
        string finalPath = System.IO.Path.Combine(pathText.text, nameText.text);
        int width = int.Parse(widthText.text);
        int height = int.Parse(heightText.text);
        screenshotCamera.SaveScreenshot(finalPath, width, height, transBg.isOn);
        gameObject.SetActive(false);
    }
}
