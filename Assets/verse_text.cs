using UnityEngine;

public class verse_text : MonoBehaviour {

    public string verse;
    private GameObject foreground;
    private GameObject shadow;

    private GameObject generateText(string name, Color c) {
        GameObject g = new GameObject(name);
        g.transform.parent = gameObject.transform;
        g.layer = 8;
        TextMesh text = g.AddComponent<TextMesh>();
        text.transform.localPosition = Vector3.zero;
        text.transform.localScale = Vector3.one * .1f;
        text.fontSize = 142;
        text.text = verse;
        text.color = c;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        return g;
    }

	public void Start () {

        foreground = generateText("Foreground", Color.white);
        shadow = generateText("Shadow", Color.black);
        shadow.transform.localPosition += shadow.transform.forward*.1f;
	}
	
	public void Update () {
        Vector3 v = new Vector3(Mathf.Sin(Time.time)/4f, Mathf.Cos(Time.time), 0);
        foreground.transform.localPosition = v;
        shadow.transform.localPosition = v + shadow.transform.forward*.1f;
    }

    public void OnDestroy() {
        Destroy(foreground);
        Destroy(shadow);
    }
}
