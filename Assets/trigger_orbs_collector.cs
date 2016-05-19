using UnityEngine;
using System.Collections;

public class trigger_orbs_collector : MonoBehaviour {

	internal void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag != "orb") return;

        Transform from_text = c.transform.parent.parent;

        Debug.Log("ORB COLLECTED! text: " + from_text.name);
	}
}
