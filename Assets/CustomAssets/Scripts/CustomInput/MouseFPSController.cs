using UnityEngine;
using System.Collections;

public class MouseFPSController : MonoBehaviour {
    private Vector3 p0, p1;
    public float sensibility = 4f;

    public void Start () {
	    Cursor.lockState = CursorLockMode.Confined;
	}
	

	public void Update () {
	    doMouseMovement();
	}

    private void doMouseMovement() {
        Vector3 mouse = Input.mousePosition;
        p0 = new Vector3(mouse.x / Screen.width * 2 - 1, mouse.y / (2 * Screen.height) * 2 - 1, 0);
        Debug.Log(p0);
        Vector3 dif = p0 - p1;
        //Camera.main.transform.localPosition += new Vector3(dif.x, 0, 0);
        Camera.main.transform.Rotate(new Vector3(-sensibility * dif.y, sensibility * dif.x, 0), Space.Self);
        p1 = p0;
    }
}
