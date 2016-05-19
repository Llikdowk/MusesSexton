using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.CustomAssets.Scripts.Components;

public class SuperTestSet : MonoBehaviour {

    private List<TextSetComponent> allTextSets = new List<TextSetComponent>();
    
    public void Start() {
        foreach (Transform child in transform) {
            allTextSets.Add(child.GetChild(0).GetComponent<TextSetComponent>());
        }
    }

    /*
    public void updateTextSetGenders() {
        foreach (TextSetComponent t in allTextSets) {
            t.updateTextGenders();
        }
    }
    */
}
