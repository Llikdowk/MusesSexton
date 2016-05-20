using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;

public class trigger_cart_drive : MonoBehaviour {

	public void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().insideCartDrive = true;
    }

    public void OnTriggerStay(Collider c) {
        if (c.gameObject.tag != "Player") return;
        if (Player.getInstance().behaviour.GetType() != typeof(DriveCartBehaviour)) { UIUtils.drive(); }
    }

    public void OnTriggerExit(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().insideCartDrive = false;
        UIUtils.markToClear();
    }
}
