using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour {
    #region Singleton
    private static CharacterCreation _instance;
    public static CharacterCreation Instance { get { return _instance; } }
    #endregion

    public static GameObject hair;
    public static int ccHairIndex;

    public Sprite messyBlondeHair;

    #region Singleton
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateHair()
    {
        switch (ccHairIndex) {
            case 0:
                hair.GetComponent<Image>().sprite = messyBlondeHair;
                break;

            case 1:
                hair.GetComponent<Image>().sprite = null;
                break;
        }
    }
}
