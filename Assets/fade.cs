using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fade : MonoBehaviour {

    public Image[] imgs;
    public Text[] texts;

	public void fadeOut() {
	    foreach(Image img in imgs) {
            StartCoroutine(runFadeOut(img));
	    }
        foreach(Text text in texts) {
            StartCoroutine(runFadeOut(text));
        }
	}

    public IEnumerator runFadeOut(MaskableGraphic uiObject, float k = 0.1f) {
        while (uiObject.color.a > 0) {
            uiObject.color = new Color(uiObject.color.r, uiObject.color.g, uiObject.color.b, uiObject.color.a - k);
            yield return new WaitForSeconds(0.016f);
        }
    }
}
