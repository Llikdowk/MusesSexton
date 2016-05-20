using System;
using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.Audio;

public class TombstoneController : MonoBehaviour {
    private readonly TextMesh[] textTombstone = new TextMesh[3];
    private int count = 0;
    internal void Start() {
        textTombstone[0] = transform.GetChild(0).GetComponent<TextMesh>();
        textTombstone[1] = transform.GetChild(1).GetComponent<TextMesh>();
        textTombstone[2] = transform.GetChild(2).GetComponent<TextMesh>();
    }

    public void goUp(string verse, int index) {
        textTombstone[index].text = split(verse, index);
        //transform.position += Vector3.up;
        AudioUtils.playTombstoneUp();
        StartCoroutine(doActionUp());
    }

    private IEnumerator doActionUp() {
        Vector3 origin = transform.position;
        Vector3 destination;
        if (count == 0) {
            destination = transform.position + Vector3.up * 1.5f;
        } else {
            destination = transform.position + Vector3.up * 0.75f;
        }
        float t = 0.0f;
        while (t < 1.0f) {
            transform.position = Vector3.Slerp(origin, destination, t);
            t += 0.0036f;
            yield return new WaitForFixedUpdate();
        }
        ++count;
    }

    private static string split(string text, int index) {
        if (index == 0) {
            if (text.Length < 18) return text;

            int splitIndex = text.IndexOf(" ", text.Length / 2, StringComparison.Ordinal);
            return text.Substring(0, splitIndex) + '\n' + text.Substring(splitIndex + 1);
        }
        else {
            if (text.Length < 28) return text;

            int splitIndex = text.LastIndexOf(" ", text.Length / 2, StringComparison.Ordinal);
            return text.Substring(0, splitIndex) + '\n' + text.Substring(splitIndex + 1);
        }
    }
}
