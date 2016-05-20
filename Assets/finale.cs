using UnityEngine;
using System.Collections;

public class finale : MonoBehaviour {

	public void OnTriggerEnter(Collider c) {
        Debug.Log("has entered1!");
        RenderSettings.fogDensity = 1.0f;
    }
}
