﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LampType { FLICKERING, LOOSE };

public class LampManager : MonoBehaviour {
    #region Singleton
    private static LampManager _instance;
    public static LampManager Instance { get { return _instance; } }
    #endregion

    public LampType selectedLampType;

    bool memberInvoked;

    [HideInInspector]
    public float swingForce;    // VERWERK DEZE VARIABLE IN HET EDITOR SCRIPT.

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

        yield return new WaitForSeconds(1);
        transform.parent.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 5));
        yield return new WaitForSeconds(1);
        transform.parent.GetComponent<Rigidbody>().AddForce(Vector3.zero);
        yield return new WaitForSeconds(1);


        memberInvoked = false;
    }
}


