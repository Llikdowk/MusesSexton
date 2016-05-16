using Assets;
using Assets.CustomAssets.Scripts.Player;
using Cubiquity;
using UnityEngine;

public class wait_till_load : MonoBehaviour {

    public fade UICurtain;
    public TerrainVolume terrain;  
    private PlayerController controller;
    private bool alreadyLoaded = false;

	public void Start () {
        UICurtain.gameObject.SetActive(true);
        controller = gameObject.GetComponent<PlayerController>();
	}
	
	public void Update () {
	    if (!alreadyLoaded && terrain.isMeshSyncronized) {
            controller.enabled = true;
            alreadyLoaded = true;
            UICurtain.fadeOut();
	    }
	}
}
