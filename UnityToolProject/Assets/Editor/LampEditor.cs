using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LampEditor : EditorWindow {

    string[] lightTypes = new string[] { "Flickering Light", "Swinging Light", "Normal Light" };
    int lightIndex;

    float swingForce;
    float intensity;

    bool lampVisible;

    GameObject lampModel;

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

        lampVisible = GUILayout.Toggle(lampVisible, "Visible?");
        if (lampVisible) {
            lampModel = (GameObject)EditorGUILayout.ObjectField(lampModel, typeof(GameObject), false);
        }

        GUILayout.Space(5f);

        switch (lightIndex) {
            case 0:
                GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                intensity = EditorGUILayout.FloatField("Intensity", intensity);

                break;

            case 1:
                GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                swingForce = EditorGUILayout.FloatField("Swing Force", swingForce);
                intensity = EditorGUILayout.FloatField("Intensity", intensity);

                break;
        }

        GUILayout.Space(10f);

        if (GUILayout.Button("Create Lamp")) {

            switch (lightIndex) {
                case 0: // Creates the Flickering Light (Instantiate Prefab)
                    GameObject flickeringLight = Instantiate(Resources.Load("fLight") as GameObject,
                                                             Vector3.zero, 
                                                             Quaternion.Euler(90, 0, 0));
                    if (lampVisible) {
                        GameObject lamp = Instantiate(lampModel as GameObject, Vector3.zero, Quaternion.identity);
                        lamp.transform.parent = flickeringLight.transform;
                    }

                    flickeringLight.name = "New Flickering Light";
                    flickeringLight.GetComponent<Light>().intensity = intensity;
                    break;

                case 1:// Creates the Swinging Light (Instantiate Prefab)
                    GameObject swingingLight = Instantiate(Resources.Load("sLight") as GameObject,
                                                           Vector3.zero,
                                                           Quaternion.identity);

                    swingingLight.name = "New Swinging Light";
                    if (lampVisible) {
                        GameObject lamp = Instantiate(lampModel as GameObject, Vector3.zero, Quaternion.identity);
                        lamp.transform.parent = swingingLight.transform.GetChild(0).transform.GetChild(0).transform;
                        lamp.transform.position = lamp.transform.parent.transform.position;
                    }
                    
                    swingingLight.transform.GetChild(0).transform.GetChild(0).GetComponent<Light>().intensity = intensity;
                    swingingLight.transform.GetChild(0).transform.GetChild(0).GetComponent<LampManager>().swingForce = swingForce;
                    break;

                case 2: // Creates the Normal Light (Instantiate Prefab)
                    GameObject normieLamp = new GameObject("New Normal Spotlight");
                    Light lt = normieLamp.AddComponent<Light>();
                    lt.type = LightType.Spot;

                    if (GameObject.Find("Lights") != null) lt.transform.parent = GameObject.Find("Lights").transform;
                    break;
            }
        }
    }
}