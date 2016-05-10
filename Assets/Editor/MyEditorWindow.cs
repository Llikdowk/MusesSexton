using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEditor;
using Assets.CustomAssets.Scripts.DraggableObject;

namespace Assets.Editor {

    public class MyEditorWindow : EditorWindow {
        private List<DataObject> data = new List<DataObject>();
        private bool doRepaint = false;
        private Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);
        private float scale = 2;

        public MyEditorWindow() {
            data.Add(new DataObject("One", 1, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Two", 2, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Three", 3, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Four", 4, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
            data.Add(new DataObject("Five", 5, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        }

        [MenuItem("Custom/Dialogues")]
        public static void Launch() {
            GetWindow(typeof(MyEditorWindow)).Show();
        }

        public void Update() {
            if (doRepaint) {
                Repaint();
                doRepaint = false;
            }
        }

        public void OnGUI() {
            DataObject dropDead;
            GUI.Box(dropTargetRect, "Die");
            var toFront = dropDead = null;
            //bool flipRepaint = false;
            foreach (DataObject d in data) {
                bool previousState = d.dragging;
                Color color = GUI.color;
                if (previousState) {
                    GUI.color = dropTargetRect.Contains(Event.current.mousePosition) ? Color.red : color;
                }

                d.draw(scale);

                GUI.color = color;
                if (d.dragging) {
                    doRepaint = true;
                    if (data.IndexOf(d) != data.Count - 1) {
                        toFront = d;
                    }
                } else if (previousState) {
                    //flipRepaint = true;
                    if (dropTargetRect.Contains(Event.current.mousePosition)) {
                        dropDead = d;
                    }
                }
            }

            if (EventUtils.mouseWheelUp()) {
                scale = Mathf.Min( 3f, scale + .25f);
                doRepaint = true;
            }
            else if (EventUtils.mouseWheelDown()) {
                scale = Mathf.Max(1, scale - .25f);
                doRepaint = true;
            }


            if (toFront != null) { // Move an object to front if needed
                data.Remove(toFront);
                data.Add(toFront);
                toFront.isFront = true;
            }

            if (dropDead != null) { // Destroy an object if needed
                data.Remove(dropDead);
            }

            /*
            if (flipRepaint) { // If some object just stopped being dragged, we should repaing for the state change
                Repaint();
            }
            */

        }
    }
}
