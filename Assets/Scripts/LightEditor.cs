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
using IVLab.ABREngine;
using Newtonsoft.Json.Linq;

public class LightEditor : MonoBehaviour
{
    // Array of light editing tiles used to edit each individual light
    [SerializeField] private LightEditorTile[] lightEditors;

    private bool initDefaultLightYet = false;

    // Start is called before the first frame update
    void Start()
    {
        ABREngine.Instance.OnStateChanged += OnABRStateChanged;
    }

    // Ensures the light editors are consistent with the state whenever it changes
    void OnABRStateChanged(JObject newState)
    {
        GameObject lightParent = GameObject.Find("ABRLightParent");
        if (!initDefaultLightYet)
        {
            if (lightParent == null || lightParent.transform.childCount == 0)
            {
                // Initialize a new light
                lightEditors?[0].CreateLight();
            }
            else
            {
                UpdateEditorTiles(lightParent);
            }
            initDefaultLightYet = true;
        }
        else if (lightParent != null)
        {
            UpdateEditorTiles(lightParent);
        }
    }

    // Updates editor tiles to match whatever lights are currently in the scene
    private void UpdateEditorTiles(GameObject lightParent)
    {
        // Determine which lights are new and need an editor, and which
        // already have existing editors
        List<int> existingEditors = new List<int>();
        List<GameObject> newLights = new List<GameObject>();
        foreach (Transform lightTransform in lightParent.transform)
        {
            bool newLight = true;
            string lightName = lightTransform.gameObject.name;
            for (int i = 0; i < lightEditors.Length; i++)
            {
                if (lightName == lightEditors[i].lightName)
                {
                    existingEditors.Add(i);
                    lightEditors[i].EnableWithLight(lightName);
                    newLight = false;
                    break;
                }
            }
            if (newLight)
            {
                newLights.Add(lightTransform.gameObject);
            }
        }

        // Generate an array of available editors
        List<int> availableEditors = new List<int>();
        for (int i = 0; i < lightEditors.Length; i++)
        {
            if (!existingEditors.Contains(i))
            {
                availableEditors.Add(i);
            }
        }

        // Attach a new light to each of the available editors,
        // disabling any editors that remain
        for (int i = 0; i < availableEditors.Count; i++)
        {
            if (i < newLights.Count)
            {
                lightEditors[availableEditors[i]].EnableWithLight(newLights[i].name);
            }
            else
            {
                lightEditors[availableEditors[i]].Disable();
            }
        }
    }
    
    // Limits active editing to only one editor at a time
    public void LimitEditing(LightEditorTile mainLightEditor)
    {
        foreach (LightEditorTile lightEditor in lightEditors)
        {
            if (lightEditor != mainLightEditor)
            {
                lightEditor.SaveAndClose();
            }
        }
    }
}
