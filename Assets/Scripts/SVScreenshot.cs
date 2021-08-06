/* SVScreenshot.cs
 *
 * Copyright (c) 2021, University of Minnesota
 * Author: Bridger Herman <herma582@umn.edu>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVLab.Utilities;
using TMPro;

public class SVScreenshot : MonoBehaviour
{
    public TMP_InputField pathText;
    public TMP_InputField nameText;
    public TMP_InputField widthText;
    public TMP_InputField heightText;
    public Toggle transBg;
    public Screenshot screenshotCamera;

    void Start()
    {
        var homevar = "";
        if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
            homevar = "USERPROFILE";
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
        screenshotCamera.SaveScreenshotOnLateUpdate(finalPath, width, height, transBg.isOn);
        gameObject.SetActive(false);
    }
}
