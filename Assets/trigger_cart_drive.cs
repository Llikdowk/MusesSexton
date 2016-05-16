using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts;
using Assets.CustomAssets.Scripts.Player;

public class trigger_cart_drive : MonoBehaviour {

	public void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().insideCartDrive = true;
    }

    public void OnTriggerExit(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().insideCartDrive = false;
    }
}
