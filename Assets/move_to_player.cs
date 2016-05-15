using UnityEngine;
using System.Collections;

public class move_to_player : MonoBehaviour {

    public float movementSpeed = 0.032f;
    private readonly Vector3 nearScale = new Vector3(.05f, .05f, .05f);
    private Transform destination;
    private bool actionDone = false;

	public void doAction(Transform destination) {
        if (!actionDone) {
            this.destination = destination;
            StartCoroutine(run());
            actionDone = true;
        }
	}

    private IEnumerator run() {
        int n = 0;
        foreach (Transform verse in transform) {
            StartCoroutine(move(verse, n));
            ++n;
            yield return new WaitForSeconds(.25f);
        }
    }

    private IEnumerator move(Transform verse, int n) {
        float t = 0f;
        Vector3 originalScale = verse.localScale;
        Vector3 originalPosition = verse.position;
        Quaternion originalRotation = verse.rotation;
        while (t < 1f) {
            t += movementSpeed;
            verse.position = Vector3.Slerp(originalPosition, destination.GetChild(n).position, t);
            verse.rotation = Quaternion.Slerp(originalRotation, destination.rotation, t);
            verse.localScale = Vector3.Slerp(originalScale, nearScale, t);
            yield return new WaitForSeconds(.016f);
        }
    }
}
