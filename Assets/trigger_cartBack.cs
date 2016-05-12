using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;

public class trigger_cartBack : MonoBehaviour {
    /*
	public void OnTriggerStay(Collider c) {
        if (c.gameObject.tag != "Player") return;

        if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
            //Player.getInstance().behaviour = new CoffinDragBehaviour(Player.getInstance().gameObject);
            Player.getInstance().triggerCartBack = true;
        }
	}

    public void OnTriggerExit(Collider c) {
        if (c.gameObject.tag != "Player") return;
        Player.getInstance().triggerCartBack = false;
    }
    */
}
