using Assets;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player;
using Cubiquity;
using UnityEngine;

public class wait_till_load : MonoBehaviour {

    //public GameObject[] physicsObjects;
    public fade UItitle;
    public fade UICurtain;
    public TerrainVolume terrain;  
    private PlayerController controller;
    private bool alreadyLoaded = false;
    private bool alreadyClicked = false;

    internal void Awake() {
        //Appli/*cation.targetFrameRate = 120;
        /*
        foreach (var obj in physicsObjects) {
            obj.SetActive(false);
        }
        */
    }

	public void Start () {
        UICurtain.gameObject.SetActive(true);
        UItitle.gameObject.SetActive(true);
        controller = gameObject.GetComponent<PlayerController>();
	}
	
	public void Update () {
        if (!alreadyClicked && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
            UItitle.fadeOut();
            alreadyClicked = true;
            if (alreadyLoaded) {
                controller.enabled = true;
            }
            return;
        }

	    if (!alreadyLoaded && terrain.isMeshSyncronized) {
            alreadyLoaded = true;
            /*
            foreach(var obj in physicsObjects) {
                obj.SetActive(true);
            }*/
            
            UICurtain.fadeOut();
            if (alreadyClicked) {
                controller.enabled = true;
            }

        }
    }
}
