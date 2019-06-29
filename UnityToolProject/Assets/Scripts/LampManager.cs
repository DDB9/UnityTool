using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampManager : MonoBehaviour 
{

    public bool flickerLights = false;

    bool cooldown = false;

    // Use this for initialization
    void Update()
    {
        if (!cooldown) // Make sure the coroutine isn't started 60 times / second.
        {
            if (flickerLights) StartCoroutine("FlickerLights");
            cooldown = true;
        }
    }

    IEnumerator FlickerLights()
    {
        // Flicker them lights.
        this.gameObject.GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(Random.Range(0.1f, 1.5f)); // Flickering interval.
        this.gameObject.GetComponent<Light>().enabled = false;

        cooldown = false;
    }
}
