/* LightEditor.cs
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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVLab.Utilities;
using IVLab.ABREngine;
using Newtonsoft.Json.Linq;

public class LightEditor : MonoBehaviour
{
    public Light editingLight = null;

    private int numLights = 0;
    bool initDefaultLightYet = false;

    // Start is called before the first frame update
    void Start()
    {
        ABREngine.Instance.OnStateChanged += OnABRStateChanged;
    }

    void OnABRStateChanged(JObject newState)
    {
        if (!initDefaultLightYet)
        {
            GameObject lightParent = GameObject.Find("ABRLightParent");
            if (lightParent == null || lightParent.transform.childCount == 0)
            {
                // Initialize a new light
                NewLight(string.Format("Light {0}", numLights + 1));
            }
            SaveLights();
            initDefaultLightYet = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lightParent = GameObject.Find("ABRLightParent");

        if (lightParent == null)
        {
            return;
        }

        numLights = 0;

        // Update light list from state
        Dropdown lightOptions = GetComponentInChildren<Dropdown>();
        if (lightOptions != null)
        {
            int selected = lightOptions.value;
            lightOptions.ClearOptions();

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (Transform light in lightParent.transform)
            {
                options.Add(new Dropdown.OptionData {
                    text = light.gameObject.name,
                });
                numLights += 1;
            }
            options.Reverse();
            lightOptions.AddOptions(options);
            lightOptions.value = selected;
        }
    }

    public void LightIntensity(float value)
    {
        editingLight.intensity = value;
    }

    public void LightName(string name)
    {
        editingLight.gameObject.name = name;
    }

    public void NewLight(string name)
    {
        if (name.Length == 0)
        {
            name = string.Format("Light {0}", numLights + 1);
        }

        GameObject lightParent = GameObject.Find("ABRLightParent");
        if (lightParent == null)
        {
            lightParent = new GameObject("ABRLightParent");
            lightParent.transform.parent = GameObject.Find("ABREngine").transform;
        }

        GameObject newLight = new GameObject(name);
        newLight.transform.parent = lightParent.transform;
        Light l = newLight.AddComponent<Light>();
        l.type = LightType.Directional;
        l.shadows = LightShadows.None;
    }

    public void DeleteLight()
    {
        Dropdown lightOptions = GetComponentInChildren<Dropdown>();
        GameObject light = GameObject.Find(lightOptions.captionText.text);
        if (light == null || !light.TryGetComponent(out editingLight))
        {
            Debug.LogErrorFormat("No light named {0}", lightOptions.captionText.text);
            return;
        }
        Destroy(GameObject.Find(lightOptions.captionText.text));
    }

    public void EditLight() {
        Dropdown lightOptions = GetComponentInChildren<Dropdown>();
        Text lightNameText = GetComponentsInChildren<Text>().Where((c) => c.gameObject.name == "LightNameText").First();
        Slider intensitySlider = GetComponentInChildren<Slider>();

        GameObject light = GameObject.Find(lightOptions.captionText.text);
        if (light == null || !light.TryGetComponent(out editingLight))
        {
            Debug.LogErrorFormat("No light named {0}", lightOptions.captionText.text);
            return;
        }

        lightNameText.text = editingLight.name;
        intensitySlider.value = editingLight.intensity;

        // Disable the camera control
        ClickAndDragCamera cameraParent = GameObject.Find("Camera Parent").GetComponentInChildren<ClickAndDragCamera>();
        cameraParent.enabled = false;

        ClickAndDragRotation rot = null;
        if (!editingLight.gameObject.TryGetComponent<ClickAndDragRotation>(out rot))
        {
            rot = editingLight.gameObject.AddComponent<ClickAndDragRotation>();
        }
    }

    public void SaveLights()
    {
        // Re-enable the camera control
        ClickAndDragCamera cameraParent = GameObject.Find("Camera Parent").GetComponentInChildren<ClickAndDragCamera>();
        cameraParent.enabled = true;

        // Disable the light control
        if (editingLight != null)
        {
            Destroy(editingLight.GetComponent<ClickAndDragRotation>());
            editingLight = null;
        }

        ABREngine.Instance.SaveStateAsync();
    }
}
