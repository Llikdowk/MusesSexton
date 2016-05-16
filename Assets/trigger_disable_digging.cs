using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.Player;

public class trigger_disable_digging : MonoBehaviour {

    public void OnTriggerEnter(Collider c) {
        if (c.tag != "Player") return;
        Player.getInstance().digNewHolesDisabled = true;
        Debug.Log("carve new hollows disabled");
    }

    public void OnTriggerExit(Collider c) {
        if (c.tag != "Player") return;
        Player.getInstance().digNewHolesDisabled = false;
        Debug.Log("carve new hollows enabled");
    }
}
