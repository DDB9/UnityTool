using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(CharacterCreation))]
public class RPGEditor : Editor
{

    string[] hairColor = new string[] { "Red", "Blue", "Green", "Blonde", "Black" };    // Possible hair colors.
    string[] hairStyle = new string[] { "Messy", "Punk", "Slick" };                     // Possible hairstyles.
    int hairStyleIndex = 0;     // Current hairstyle.
    int hairColorIndex = 0;     // Current hair color.

    string[] clothes = new string[] { "Suit", "Tanktop", "Casual" };    // Possible clothing styles.
    int clothingIndex = 0;      // Current clothing style.

    string characterBio = "";   // Custom user-writeen character biography.

    Color armColor;             // Color of (both) arms.

    public override void OnInspectorGUI()
    {
        Repaint();

        // Finds all the gameobjects the user can customize. Used find instead of public variables to ease the usage of the tool.
        #region GameObject Collection
        Image hair = GameObject.Find("rpgtool_HAIR").GetComponent<Image>();
        Image body = GameObject.Find("rpgtool_BODY").GetComponent<Image>();
        Image rightLeg = GameObject.Find("rpgtool_RIGHTFOOT").GetComponent<Image>();
        Image leftLeg = GameObject.Find("rpgtool_LEFTFOOT").GetComponent<Image>();
        Image rightArm = GameObject.Find("rpgtool_RIGHTARM").GetComponent<Image>();
        Image leftArm = GameObject.Find("rpgtool_LEFTARM").GetComponent<Image>();
        Image rightShoe = GameObject.Find("rpgtool_RIGHTSHOE").GetComponent<Image>();
        Image leftShoe = GameObject.Find("rpgtool_LEFTSHOE").GetComponent<Image>();

        armColor = GameObject.Find("rpgtool_LEFTHAND").GetComponent<Image>().color;
        #endregion

        GUILayout.Label("[+] CREATE YOUR CHARACTER [+]", EditorStyles.boldLabel);

        GUILayout.Space(15f);

        // Change hairstyle -and color
        GUILayout.Label("HAIR", EditorStyles.boldLabel);
        hairColorIndex = EditorGUILayout.Popup("Select Haircolor:", hairColorIndex, hairColor);
        hairStyleIndex = EditorGUILayout.Popup("Select HairStyle:", hairStyleIndex, hairStyle);

        // Button to update your character.
        #region Character Update
        if (GUILayout.Button("Update Hair style"))
        {
            switch (hairStyleIndex)
            {
                case 0:
                    hair.sprite = Resources.Load<Sprite>("Hairstyles/MessyHair");
                    break;
                case 1:
                    hair.sprite = Resources.Load<Sprite>("Hairstyles/PunkHair");
                    break;
                case 2:
                    hair.sprite = Resources.Load<Sprite>("Hairstyles/SlickHair");
                    break;
            }

            switch (hairColorIndex)
            {
                case 0:
                    Color tempr = Color.red;
                    hair.color = tempr;
                    break;
                case 1:
                    Color tempb = Color.blue;
                    hair.color = tempb;
                    break;
                case 2:
                    Color tempg = Color.green;
                    hair.color = tempg;
                    break;
                case 3:
                    Color tempy = Color.yellow;
                    hair.color = tempy;
                    break;
                case 4:
                    Color tempbl = Color.black;
                    hair.color = tempbl;
                    break;
            }
            SaveData(0);
        }

        #endregion


        // Change clothes.
        GUILayout.Space(10f);
        GUILayout.Label("CLOTHES", EditorStyles.boldLabel);
        clothingIndex = EditorGUILayout.Popup("Select Clothes:", clothingIndex, clothes);

        if (GUILayout.Button("Update Clothing"))
        {
            switch (clothingIndex)
            {
                case 0: // suit
                    body.sprite = null;
                    body.color = Color.black;
                    rightLeg.color = Color.black;
                    leftLeg.color = Color.black;
                    rightArm.color = Color.black;
                    leftArm.color = Color.black;
                    rightShoe.color = Color.grey;
                    leftShoe.color = Color.grey;
                    break;

                case 1: // summer clothing
                    body.color = Color.white;
                    body.sprite = Resources.Load<Sprite>("Clothing/TankTop");
                    rightLeg.color = Color.cyan;
                    leftLeg.color = Color.cyan;
                    rightArm.color = armColor;
                    leftArm.color = armColor;
                    rightShoe.color = Color.white;
                    leftShoe.color = Color.white;
                    break;

                case 2: // casual clothing
                    body.sprite = null;
                    body.color = Color.white;
                    rightLeg.color = Color.blue;
                    leftLeg.color = Color.blue;
                    rightArm.color = armColor;
                    leftArm.color = armColor;
                    rightShoe.color = Color.white;
                    leftShoe.color = Color.white;
                    break;
            }
            SaveData(1);
        }

        GUILayout.Space(10f);

        // The user can write their character's biography if they like. Gimmick feature.
        GUILayout.Label("CHARACTER BIO");
        EditorGUILayout.TextField("Enter a biography for your character.");

        if (GUILayout.Button("Save Character Bio"))
        {   // Saves the bio to the character.
            SaveData(2);
        }

        GUILayout.Space(15f);

        // Green button to save all character changes at once.
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SAVE ALL CHANGES"))
        {
            SaveData(3);
        }

        GUILayout.Space(5f);

        // Blue button to load an already saved character into the tool.
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Load Character"))
        {
            LoadData(3);
        }
        GUILayout.Label("If you have a character saved from a different project, please navigate to \n" +
            "C:/Users/[Computer Name]/AppData/LocalLow/[Company Name]/[Project Name]/rpgcharacter.data \n" +
            "and copy that file into this project's folder in the same LocalLow folder. \n" +
            "For Questions and support please send an email to ddebruijn9@gmail.com.");

        GUILayout.Space(15f);

        // Reset's the character to bare minimum. Useful for if the tool acts up.
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Reset Character"))
        {
            hair.color = Color.white;
            body.sprite = null;
            hair.sprite = null;
            body.color = armColor;
            rightLeg.color = armColor;
            leftLeg.color = armColor;
            rightArm.color = armColor;
            leftArm.color = armColor;
            rightShoe.color = armColor;
            leftShoe.color = armColor;

            hairColorIndex = 0;
        }
        GUILayout.Label("Click here if the tool is acting up.");
    }

    #region binary data safe
    // This takes care of saving the characters via a binary formatter.
    public void SaveData(int dataType)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        CharacterData data = new CharacterData();

        FileStream file = File.Create(Application.persistentDataPath + "/rpgcharacter.RPGTool");    // Save location.
        // Writes all changed data to the file.
        switch (dataType)
        {
            case 0:
                data.hairColorIndex = hairColorIndex;
                data.hairStyleIndex = hairStyleIndex;
                break;
            case 1:
                data.clothingIndex = clothingIndex;
                break;
            case 2:
                data.characterBio = characterBio;
                break;
            case 3:
                data.hairColorIndex = hairColorIndex;
                data.hairStyleIndex = hairStyleIndex;
                data.clothingIndex = clothingIndex;
                data.characterBio = characterBio;
                break;
        }

        formatter.Serialize(file, data);
        file.Close();
        Debug.Log("Character saved to " + Application.persistentDataPath + "/rpgcharacter.RPGTool");
    }

    /* This takes care of loading already exsisting characters into the tool.
     * This basically does almost the exact same as the save function, but in reverse. So instead of writing the local variables to the file,
     * this function writes the variables from the file onto the local variables. */
    public void LoadData(int dataType)
    {
        if (File.Exists(Application.persistentDataPath + "/rpgcharacter.RPGTool"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/rpgcharacter.RPGTool", FileMode.Open);

            file.Position = 0;
            CharacterData data = (CharacterData)formatter.Deserialize(file);
            file.Close();

            switch (dataType)
            {
                case 0:
                    hairColorIndex = data.hairColorIndex;
                    hairStyleIndex = data.hairStyleIndex;
                    break;
                case 1:
                    clothingIndex = data.clothingIndex;
                    break;
                case 2:
                    characterBio = data.characterBio;
                    break;
                case 3:
                    hairColorIndex = data.hairColorIndex;
                    hairStyleIndex = data.hairStyleIndex;
                    clothingIndex = data.clothingIndex;
                    characterBio = data.characterBio;
                    break;
            }

            Debug.Log("Character loaded from " + Application.persistentDataPath + "/rpgcharacter.data");
        }
        else
        {
            Debug.LogError("Failed to load due to the abscense of rpgcharacter.data file.");
        }
    }
}

[Serializable]
class CharacterData
{
    public int hairColorIndex;
    public int hairStyleIndex;
    public int clothingIndex;
    public string characterBio;
}
#endregion
