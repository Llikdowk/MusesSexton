using Assets;
using Assets.CustomAssets.Scripts.Player;
using Cubiquity;
using UnityEngine;

public class wait_till_load : MonoBehaviour {

    public GameObject[] physicsObjects;
    public fade UICurtain;
    public TerrainVolume terrain;  
    private PlayerController controller;
    private bool alreadyLoaded = false;

    internal void Awake() {
        foreach (var obj in physicsObjects) {
            obj.SetActive(false);
        }
    }

	public void Start () {
        UICurtain.gameObject.SetActive(true);
        controller = gameObject.GetComponent<PlayerController>();
	}
	
	public void Update () {
	    if (!alreadyLoaded && terrain.isMeshSyncronized) {
            controller.enabled = true;
            alreadyLoaded = true;
            foreach(var obj in physicsObjects) {
                obj.SetActive(true);
            }
            UICurtain.fadeOut();
	    }
	}
}
