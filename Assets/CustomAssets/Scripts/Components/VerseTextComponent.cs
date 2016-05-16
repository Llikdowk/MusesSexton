using UnityEngine;

//should be renamed to: VerseTextComponent
public class VerseTextComponent : MonoBehaviour {

    public string verse;
    private GameObject foreground;
    private GameObject shadow;

    public Color color;
    public Color shadowColor;
    public Color overColor;

    public Vector3 originalPosition { get; private set; }
    public Quaternion originalRotation { get; private set; }
    public Vector3 originalScale { get; private set; }

    public void resetOrigins() {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }

    public void setOverColor() {
        foreground.GetComponent<TextMesh>().color = overColor;
    }

    public void setNormalColor() {
        foreground.GetComponent<TextMesh>().color = color;
    }

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
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        foreground = generateText("Foreground", Color.white);
        shadow = generateText("Shadow", Color.black);
        shadow.transform.localPosition += shadow.transform.forward*.1f;
	}
	
	public void Update () {
        //Vector3 v = new Vector3(Mathf.Sin(Time.time)/4f, Mathf.Cos(Time.time), 0);
        //foreground.transform.localPosition = v;
        //shadow.transform.localPosition = v + shadow.transform.forward*.1f;
    }

    public void OnDestroy() {
        Destroy(foreground);
        Destroy(shadow);
    }
}
