/* BackgroundColor.cs
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
        ABREngine.Instance.SaveStateAsync<HttpStateFileLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        mainCam.backgroundColor = picker.Color;
    }
}
