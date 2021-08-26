using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IVLab.Utilities;
using IVLab.ABREngine;

public class LightEditorTile : MonoBehaviour
{
    [Header("Settings")]
    public string lightName;
    [Header("Dependencies")]
    [SerializeField] private LightEditor lightEditorController;
    [SerializeField] private GameObject newLightButton;
    [SerializeField] private Image editButtonImage;
    [SerializeField] private Sprite editIcon, checkmarkIcon;
    [SerializeField] private RectTransform intensityEditor;

    private bool editingLight = false;
    private Light myLight;

    // Toggles whether or not the light is in edit mode
    public void ToggleEditLight()
    {
        editingLight = !editingLight;

        // Editing toggled on:
        if (editingLight)
        {
            // Find the light
            GameObject light = GameObject.Find(lightName);
            if (light == null || !light.TryGetComponent(out myLight))
            {
                Debug.LogErrorFormat("No light named {0}", lightName);
                return;
            }

            // Show the intensity editor
            editButtonImage.sprite = checkmarkIcon;
            intensityEditor.offsetMin = new Vector2(intensityEditor.offsetMin.x, -25);
            Slider intensitySlider = GetComponentInChildren<Slider>();
            intensitySlider.value = myLight.intensity;

            // Disable the camera control
            ClickAndDragCamera cameraParent = GameObject.Find("Camera Parent").GetComponentInChildren<ClickAndDragCamera>();
            cameraParent.enabled = false;

            // Enable light control
            ClickAndDragRotation rot = null;
            if (!myLight.gameObject.TryGetComponent<ClickAndDragRotation>(out rot))
            {
                rot = myLight.gameObject.AddComponent<ClickAndDragRotation>();
            }
        }
        // Editing toggled off:
        else
        {
            // Re-enable camera control
            ClickAndDragCamera cameraParent = GameObject.Find("Camera Parent").GetComponentInChildren<ClickAndDragCamera>();
            cameraParent.enabled = true;

            // Disable light control
            if (myLight != null)
            {
                Destroy(myLight.GetComponent<ClickAndDragRotation>());
                myLight = null;
            }

            // Hide the intensity editor
            editButtonImage.sprite = editIcon;
            intensityEditor.offsetMin = new Vector2(intensityEditor.offsetMin.x, 0);

            // Save the changes
            SaveChanges();
        }
    }

    // Set the intensity of the light
    public void LightIntensity(float value)
    {
        myLight.intensity = value;
    }

    // Create a new light
    public void CreateLight()
    {
        // Hide the new light button
        newLightButton.SetActive(false);
        
        // Name the light
        GetComponentInChildren<TextMeshProUGUI>().text = lightName;
        // Find the light parent
        GameObject lightParent = GameObject.Find("ABRLightParent");
        if (lightParent == null)
        {
            lightParent = new GameObject("ABRLightParent");
            lightParent.transform.parent = GameObject.Find("ABREngine").transform;
        }

        // Create the light
        GameObject newLight = new GameObject(lightName);
        newLight.transform.parent = lightParent.transform;
        Light l = newLight.AddComponent<Light>();
        l.type = LightType.Directional;
        l.shadows = LightShadows.None;
        // Save the light
        SaveChanges();
    }

    // Delete the light related to this editor, if any
    public void DeleteLight()
    {
        GameObject light = GameObject.Find(lightName);
        if (light == null || light.GetComponent<Light>() == null)
        {
            Debug.LogErrorFormat("No light named {0}", lightName);
            return;
        }
        Destroy(light);
        Disable();

        SaveChanges();
    }

    // Re-enables this editor to use a different light
    public void EnableWithLight(string name)
    {
        // Apply new light name
        lightName = name;
        GetComponentInChildren<TextMeshProUGUI>().text = lightName;
        newLightButton.SetActive(false);
    }

    // Disables the editor, closing the edit menu and activating the new light button
    public void Disable()
    {
        if (editingLight)
        {
            ToggleEditLight();
        }

        // Show the "new light" button
        newLightButton.SetActive(true);
    }

    // Brings the tile out of edit mode, if it's in it, saving in the process
    public void SaveAndClose()
    {
        if (editingLight)
        {
            ToggleEditLight();
        }
    }

    public void SaveChanges()
    {
        ABREngine.Instance.SaveStateAsync<HttpStateFileLoader>();
    }
}
