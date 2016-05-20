using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;

public class open_gates : MonoBehaviour {

    private static Transform leftDoor;
    private static Transform rightDoor;

    public bool active = false;

    internal void Awake() {
        leftDoor = GameObject.Find("LeftDoor").transform;
        rightDoor = GameObject.Find("RightDoor").transform;
    }
	
	internal void Update () {

        if (GameActions.checkAction(Action.DEBUG2, Input.GetKeyDown)) {
            active = true;
        }

	    if (active) {
            openGates();
            active = false;
        }
	}

    public void openGates() {
        StartCoroutine(open(leftDoor));
        StartCoroutine(open(rightDoor));
    }

    public IEnumerator open(Transform door) {
        Quaternion originAngles = door.rotation;
        Quaternion destination = door.transform.GetChild(0).transform.rotation;
        float t = 0.0f;
        while (t < 1.0f) {
            t += 0.016f;
            door.rotation = Quaternion.Slerp(originAngles, destination, t);
            yield return new WaitForSeconds(0.016f);
        }
    }

}
