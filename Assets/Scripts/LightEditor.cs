using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVLab.Utilities;
using IVLab.ABREngine;

public class LightEditor : MonoBehaviour
{
    public Light editingLight = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lightParent = GameObject.Find("ABRLightParent");

        if (lightParent == null)
        {
            return;
        }

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
        InputField lightName = GetComponentInChildren<InputField>();
        Slider intensitySlider = GetComponentInChildren<Slider>();

        GameObject light = GameObject.Find(lightOptions.captionText.text);
        if (light == null || !light.TryGetComponent(out editingLight))
        {
            Debug.LogErrorFormat("No light named {0}", lightOptions.captionText.text);
            return;
        }

        lightName.text = lightOptions.captionText.text;
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
