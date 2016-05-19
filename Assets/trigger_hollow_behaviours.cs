using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using Assets.CustomAssets.Scripts;
using Assets.CustomAssets.Scripts.Components;

public class trigger_hollow_behaviours : MonoBehaviour {
    public AnimationCurve curve;
    [SerializeField] private Transform node;
    private GameObject groundFloor;
    private GameObject heap;
    private GameObject tombstone;
    private GameObject playerPosition;
    private Transform original;
    private Transform coffin;
    private bool hasCoffinInside = false;
    private bool hasAlreadyEnterPoem = false;
    public bool fullHollow = false;

    public void init(AnimationCurve curve, Transform node, GameObject groundFloor, GameObject heap, GameObject tombstone, GameObject playerPosition) {
        this.curve = curve;
        this.node = node;
        this.groundFloor = groundFloor;
        this.heap = heap;
        this.tombstone = tombstone;
        this.playerPosition = playerPosition;
    }

    public void OnTriggerEnter(Collider c) {
        Player.getInstance().insideThrowCoffinTrigger = true;
        Debug.Log("throw coffin enabled!");
    }

    public void OnTriggerExit(Collider c) {
        Player.getInstance().insideThrowCoffinTrigger = false;
        Debug.Log("throw coffin disabled!");
    }

    public void OnTriggerStay (Collider c) {
        if (c.tag != "Player") return;
        if (Player.getInstance().behaviour.GetType() == typeof(CoffinDragBehaviour)) {
            UIUtils.infoInteractive.text = "click to throw!";
            if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && fullHollow) {
                setup();
                if (!hasCoffinInside && coffin != null) {
                    Player.getInstance().doMovementDisplacement(playerPosition.transform);
                    StartCoroutine(doAction());
                }
            }
        }
        else if (Player.getInstance().behaviour.GetType() == typeof(ExploreWalkBehaviour)) {
            
            if (!hasAlreadyEnterPoem && hasCoffinInside) {
                UIUtils.infoInteractive.text = "undig!";
                DigBehaviour d = new DigBehaviour(Player.getInstance().gameObject, DigType.INVERSE);
                Player.getInstance().behaviour = d;
                d.init(groundFloor, heap, tombstone);
                hasAlreadyEnterPoem = true;
            }

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
        float t = 0;
        while (t < 1f) {
            float c = curve.Evaluate(t);
            coffin.position = Vector3.Slerp(original.position, node.position, t) + new Vector3(0, c, 0);
            coffin.rotation = Quaternion.Slerp(original.rotation, node.rotation, t);
            t += .1f;
            yield return new WaitForSeconds(.016f);
        }
        doFinalAction();
    }

    private void doFinalAction() {
        coffin.GetComponent<Rigidbody>().isKinematic = true;
        coffin = null;
        Player.getInstance().cinematic = false;
        Player.getInstance().behaviour = new ExploreWalkBehaviour(Player.getInstance().gameObject);
        hasCoffinInside = true;
        Debug.LogWarning("CAUTION! THIS BODY IS NO LONGER KINEMATIC!");
    }
    
}
