using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;

public class trigger_throw_coffin : MonoBehaviour {
    public Transform node1;
    public AnimationCurve curve;
    private Transform original;
    private Transform coffin;
    private bool coroutineEnd = false;
    private bool hasCoffinInside = false;
	
    public void OnTriggerEnter(Collider c) {
        if (c.tag != "Player") return;
        Debug.Log("hasEntered!");
        //Player.getInstance().disableRayExploration = true;
    }

    public void OnTriggerExit(Collider c) {
        //Player.getInstance().disableRayExploration = false;
    }

	public void OnTriggerStay (Collider c) {
        if (c.tag != "Player") return;

        if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
            setup();
            if (!hasCoffinInside && coffin != null) {
                StartCoroutine(doAction());
            }

            if (hasCoffinInside) {
                Debug.Log("DO UNDIG BEHAVIOUR!");
                Player.getInstance().behaviour = new PoemBehaviour(Player.getInstance().gameObject);
            }
        }

        if (coroutineEnd) {
            doFinalAction();
        }
	}

    private void setup() {
        if (Player.getInstance().coffinSlot.childCount > 0) {
            coffin = Player.getInstance().coffinSlot.GetChild(0);
            original = new GameObject().transform;
            original.position = coffin.position;
            original.rotation = coffin.rotation;
            coffin.parent = null;
        }
    }

    private IEnumerator doAction() {
        Player.getInstance().cinematic = true;
        float t = 0;
        while (t < 1f) {
            float c = curve.Evaluate(t);
            coffin.position = Vector3.Slerp(original.position, node1.position, t) + new Vector3(0, c, 0);
            coffin.rotation = Quaternion.Slerp(original.rotation, node1.rotation, t);
            t += .016f;
            yield return new WaitForSeconds(.016f);
        }
        coroutineEnd = true;
        yield return null;
    }

    private void doFinalAction() {
        coffin = null;
        Player.getInstance().cinematic = false;
        Player.getInstance().behaviour = new ExploreWalkBehaviour(Player.getInstance().gameObject);
        hasCoffinInside = true;
        coroutineEnd = false;
        Debug.LogWarning("CAUTION! THIS BODY IS NO LONGER KINEMATIC!");
    }
    
}
