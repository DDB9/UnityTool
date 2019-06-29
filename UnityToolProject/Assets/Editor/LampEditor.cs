﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LampEditor : EditorWindow {

    string[] lightTypes = new string[] { "Flickering Light", "Loose Light", "Normal Light" };
    int lightIndex = 0; 

    [MenuItem("Window/Lamp Editor")]
    static void ShowWindow() {
        EditorWindow lampEditor = EditorWindow.GetWindow(typeof(LampEditor));
        lampEditor.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Create a lamp", EditorStyles.boldLabel);

        GUILayout.Space(10f);

        GUILayout.Label("Lamp type:", EditorStyles.miniBoldLabel);
        lightIndex = EditorGUILayout.Popup(lightIndex, lightTypes);

        GUILayout.Space(10f);

        if (GUILayout.Button("Create Lamp"))
        {

            switch (lightIndex)
            {
                case 0: // Creates the Flickering Light (Instantiate Prefab)
                    GameObject flickeringLight = Instantiate(Resources.Load("Assets/Prefabs/fLight.prefab") 
                                                as GameObject, Vector3.zero, Quaternion.identity);

                    // if (GameObject.Find("Lights") != null) lt1.transform.parent = GameObject.Find("Lights").transform;
                    break;

                case 1:// Creates the Loose Light (Instantiate Prefab)
                    GameObject looseLight = new GameObject("New Loose Light");
                    Light lt2 = looseLight.AddComponent<Light>();
                    lt2.type = LightType.Spot;
                    lt2.range = 17f;
                    lt2.spotAngle = 63f;
                    lt2.intensity = 15f;

                    if (GameObject.Find("Lights") != null) lt2.transform.parent = GameObject.Find("Lights").transform;
                    break;

                case 2: // Creates the Normal Light (Instantiate Prefab)
                    GameObject normieLamp = new GameObject("New Normal Light");
                    Light lt3 = normieLamp.AddComponent<Light>();
                    lt3.type = LightType.Spot;

                    if (GameObject.Find("Lights") != null) lt3.transform.parent = GameObject.Find("Lights").transform;
                    break;
            }
        }
    }
}
