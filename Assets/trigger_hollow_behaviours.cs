using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using Assets.CustomAssets.Scripts;

public class trigger_hollow_behaviours : MonoBehaviour {
    public AnimationCurve curve;
    [SerializeField] private Transform node;
    private GameObject groundFloor;
    private GameObject heap;
    private GameObject tombstone;
    private Transform original;
    private Transform coffin;
    private bool coroutineEnd = false;
    private bool hasCoffinInside = false;
    private bool hasAlreadyEnterPoem = false;
    private bool fullHollow = false; // TODO!

    public void init(AnimationCurve curve, Transform node, GameObject groundFloor, GameObject heap, GameObject tombstone) {
        this.curve = curve;
        this.node = node;
        this.groundFloor = groundFloor;
        this.heap = heap;
        this.tombstone = tombstone;
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
                    StartCoroutine(doAction());
                }
            }
            if (coroutineEnd) {
                doFinalAction();
            }
        }
        else if (Player.getInstance().behaviour.GetType() == typeof(ExploreWalkBehaviour)) {

            if (!hasAlreadyEnterPoem && hasCoffinInside) {
                UIUtils.infoInteractive.text = "undig!";
                Player.getInstance().behaviour = new DigBehaviour(Player.getInstance().gameObject, groundFloor, heap, tombstone, DigType.INVERSE);
                hasAlreadyEnterPoem = true;
            }
            else if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && !hasCoffinInside && !fullHollow) {
                UIUtils.infoInteractive.text = "dig hollow!";
                Player.getInstance().behaviour = new DigBehaviour(Player.getInstance().gameObject, groundFloor, heap, tombstone);
                fullHollow = true;
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
        Player.getInstance().cinematic = true;
        float t = 0;
        while (t < 1f) {
            float c = curve.Evaluate(t);
            coffin.position = Vector3.Slerp(original.position, node.position, t) + new Vector3(0, c, 0);
            coffin.rotation = Quaternion.Slerp(original.rotation, node.rotation, t);
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
