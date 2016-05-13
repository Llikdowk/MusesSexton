using UnityEngine;
using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;

public class trigger_throw_coffin : MonoBehaviour {
    public Transform node1;
    public Transform node2;
    public AnimationCurve curve;
    private Transform original;
    private Transform coffin;
	
	public void Start () {
        coffin = GameObject.Find("Coffin_debug").transform;
        original = new GameObject().transform;
	}
	
	
    public void OnTriggerEnter(Collider c) {
        Debug.Log("hasEntered!");
    }

	public void OnTriggerStay (Collider c) {
        //Debug.Log("stay!");
	    if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
            //doAction(Time.time);
            original.position = coffin.position;
            original.rotation = coffin.rotation;
            StartCoroutine(doAction());
	    }
	}

    
    private IEnumerator doAction() {
        float t = 0;
        coffin.parent = null;
        while (t < 1f) {
            float c = curve.Evaluate(t);
            coffin.position = Vector3.Slerp(original.position, node1.position, t) + new Vector3(0, c, 0);
            coffin.rotation = Quaternion.Slerp(original.rotation, node1.rotation, t);
            t += .016f;
            yield return new WaitForSeconds(.016f);
        }
    }
    
}
