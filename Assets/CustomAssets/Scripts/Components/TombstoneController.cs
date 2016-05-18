using System;
using UnityEngine;
using System.Collections;

public class TombstoneController : MonoBehaviour {
    private readonly TextMesh[] textTombstone = new TextMesh[3];

    internal void Start() {
        textTombstone[0] = transform.GetChild(0).GetComponent<TextMesh>();
        textTombstone[1] = transform.GetChild(1).GetComponent<TextMesh>();
        textTombstone[2] = transform.GetChild(2).GetComponent<TextMesh>();
    }

    public void goUp(string verse, int index) {
        textTombstone[index].text = split(verse, index);
        transform.position += Vector3.up;
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
