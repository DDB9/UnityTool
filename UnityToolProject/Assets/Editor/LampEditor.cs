using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;


public class LampEditor : EditorWindow {

    string[] currentInterface = new string[] { "Tweak Lighting in Scene", "Create a Lightsource" };
    string[] lightTypes = new string[] { "Flickering Light", "Swinging Light", "Rotating Light", "Standard Spotlight" };
    List<string> savedLights = new List<string>();
    int interfaceIndex;
    int lightIndex;
    int currentPrefabIndex;

    float swingForce;
    float rotationSpeed = 100;
    float lightIntensity = 1;
    Color lightColor = Color.white;

    bool lampVisible;

    GameObject lampModel;

    public string lightName;
    public GameObject customLight;

    [MenuItem("Window/Lamp Editor")]
    static void ShowWindow() {
        EditorWindow lampEditor = EditorWindow.GetWindow(typeof(LampEditor));
        lampEditor.Show();
    }

    private void OnGUI() {
        interfaceIndex = EditorGUILayout.Popup("Select Interface:", interfaceIndex, currentInterface);
        switch (interfaceIndex) {
            case 0:
                GUILayout.Label("CUSTOM LIGHTING SETTINGS", EditorStyles.boldLabel);

                GUILayout.Space(5f);

                if (GUILayout.Button("Load your custom lights")) {
                    LoadLights();
                }
                GUILayout.Space(5f);

                GUILayout.Label("Save a custom Prefab");
                customLight = (GameObject)EditorGUILayout.ObjectField(customLight, typeof(GameObject), true);
                GUILayout.Space(3f);
                lightName = EditorGUILayout.TextField("Name your prefab:", lightName);

                if (GUILayout.Button("Save")) {
                    SaveLight();
                }

                GUILayout.Space(10f);
                GUILayout.Label("Saved Prefabs:", EditorStyles.boldLabel);
                currentPrefabIndex = EditorGUILayout.Popup("Choose a prefab:", currentPrefabIndex, savedLights.ToArray());

                break;

            case 1:
                GUILayout.Label("CREATE A LIGHTSOURCE", EditorStyles.boldLabel);

                GUILayout.Space(10f);

                lightIndex = EditorGUILayout.Popup("Lamp Type:", lightIndex, lightTypes);

                lampVisible = GUILayout.Toggle(lampVisible, "Visible?");
                if (lampVisible) {
                    lampModel = (GameObject)EditorGUILayout.ObjectField(lampModel, typeof(GameObject), false);
                }

                GUILayout.Space(5f);

                switch (lightIndex) {
                    case 0:
                        GUILayout.Label("A light that flickers with random intervals between the 0.1\nand 1.5 seconds.", EditorStyles.boldLabel);
                        GUILayout.Space(1f);
                        GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                        lightIntensity = EditorGUILayout.FloatField("Intensity", lightIntensity);

                        GUILayout.Space(3f);

                        lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
                        break;

                    case 1:
                        GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                        swingForce = EditorGUILayout.FloatField("Swing Force", swingForce);
                        lightIntensity = EditorGUILayout.FloatField("Intensity", lightIntensity);

                        GUILayout.Space(3f);

                        EditorGUILayout.ColorField("Light Color", Color.white);
                        break;

                    case 2:
                        GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                        lightIntensity = EditorGUILayout.FloatField("Intensity", lightIntensity);
                        rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", rotationSpeed);

                        GUILayout.Space(3f);

                        lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
                        break;

                    case 3:
                        GUILayout.Label("Lamp Options:", EditorStyles.miniBoldLabel);
                        lightIntensity = EditorGUILayout.FloatField("Intensity", lightIntensity);

                        GUILayout.Space(3f);

                        EditorGUILayout.ColorField("Light Color", lightColor);
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
                            flickeringLight.GetComponent<Light>().intensity = lightIntensity;
                            flickeringLight.GetComponent<Light>().color = lightColor;
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

                            swingingLight.transform.GetChild(0).transform.GetChild(0).GetComponent<Light>().intensity = lightIntensity;
                            swingingLight.transform.GetChild(0).transform.GetChild(0).GetComponent<LampManager>().swingForce = swingForce;
                            swingingLight.GetComponent<Light>().color = lightColor;
                            break;

                        case 2:
                            GameObject rotatingLight = Instantiate(Resources.Load("rLight") as GameObject,
                                                                   Vector3.zero,
                                                                   Quaternion.identity);

                            rotatingLight.name = "New Rotating Light";

                            break;

                        case 3: // Creates a standard spotlight (Instantiate Prefab)
                            GameObject standardSpotlight = new GameObject("New Standard Spotlight");
                            Light lt = standardSpotlight.AddComponent<Light>();
                            lt.type = LightType.Spot;
                            lt.color = lightColor;

                            if (GameObject.Find("Lights") != null)
                                lt.transform.parent = GameObject.Find("Lights").transform;

                            break;
                    }
                    break;
                }
                break;
        }
    }

    public void SaveLight() {
        if (savedLights.Contains(lightName)) {
            Debug.LogError("You already saved a prefab with the same name! Please choose a different name.");
            return;
        }
        savedLights.Add(lightName);

        BinaryFormatter formatter = new BinaryFormatter();
        SavedData data = new SavedData();

        FileStream file = File.Create(Application.persistentDataPath + "/lightdata.data");
        data.prefabName = lightName; 

        formatter.Serialize(file, data);
        file.Close();
    }


    public void LoadLights() {
        if (File.Exists(Application.persistentDataPath + "/lightdata")) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/lightdata.data", FileMode.Open);

            file.Position = 0;
            SavedData data = (SavedData)formatter.Deserialize(file);
            file.Close();

            savedLights.Add(data.prefabName);
        }
    }
}

[Serializable]
public class SavedData {
    public string prefabName;

}


