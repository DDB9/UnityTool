using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LampType { FLICKERING, LOOSE, ROTATING };

public class LampManager : MonoBehaviour {
    #region Singleton
    private static LampManager _instance;
    public static LampManager Instance { get { return _instance; } }
    #endregion

    public LampType selectedLampType;

    bool memberInvoked;

    [HideInInspector]
    public float swingForce;

    [System.NonSerialized]
    public float rotSpeed;

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

    // Use this for initialization
    void Update() {
        if (!memberInvoked && selectedLampType == LampType.FLICKERING) {
            StartCoroutine(FlickerLights());
        }

        if (!memberInvoked && selectedLampType == LampType.LOOSE) {
            StartCoroutine(SwingingLight());
        }

        if (selectedLampType == LampType.ROTATING) {
            transform.Rotate(Vector3.right * Time.deltaTime * rotSpeed);
        }

    }

    IEnumerator FlickerLights() {
        memberInvoked = true;

        // Flicker them lights.
        GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(Random.Range(0.1f, 1.5f)); // Flickering interval.
        GetComponent<Light>().enabled = false;

        memberInvoked = false;
    }

    IEnumerator SwingingLight() {
        memberInvoked = true;

        transform.parent.GetComponent<Rigidbody>().AddForce(0, 0, swingForce);
        yield return new WaitForSeconds(2);
        transform.parent.GetComponent<Rigidbody>().AddForce(0, 0, -swingForce);
        yield return new WaitForSeconds(1);
    }
}



