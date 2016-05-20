using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;

public class finale : MonoBehaviour {

    public GameObject tombstone;

	public void OnTriggerEnter(Collider c) {
        Debug.Log("has entered1!");
        StartCoroutine(doMoreFog());
        Player.getInstance().behaviour = new FinalPoemBehaviour(Player.getInstance().gameObject, tombstone);
    }

    private static IEnumerator doMoreFog() {
        float t = RenderSettings.fogDensity;
        while (t < .15f) {
            t += 0.1f * Time.deltaTime;
            RenderSettings.fogDensity = t;
            yield return new WaitForEndOfFrame();
        }
    }
}
