using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.Player;

public class trigger_front : MonoBehaviour {

	public void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().triggerCartFront = true;
	}

    public void OnTriggerExit(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().triggerCartFront = false;
    }
}
