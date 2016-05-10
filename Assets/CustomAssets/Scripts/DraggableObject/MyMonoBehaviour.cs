using UnityEngine;
using System.Collections.Generic;

namespace Assets.CustomAssets.Scripts.DraggableObject {
    /*
    public class MyMonoBehaviour : MonoBehaviour {
        private List<DataObject> data = new List<DataObject>();
        private Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);

        public void Awake() {
            data.Add(new DataObject("One", 1, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Two", 2, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Three", 3, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Four", 4, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Five", 5, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        }

        public void OnGUI() {

            GUI.Box(dropTargetRect, "Die");
            DataObject toFront, dropDead;
            toFront = dropDead = null;
            foreach (DataObject d in data) {
                Color color = GUI.color;

                if (d.dragging) {
                    GUI.color = dropTargetRect.Contains(Event.current.mousePosition) ? Color.red : color;
                }

                d.OnGUI();

                GUI.color = color;

                if (d.dragging) {
                    if (this.data.IndexOf(d) != this.data.Count - 1) {
                        toFront = d;
                    }
                }
            }

            if (toFront != null) { // Move an object to front if needed
                data.Remove(toFront);
                data.Add(toFront);
            }
        }
        
    }*/
}
