using UnityEngine;

namespace Assets.CustomAssets.Scripts.DraggableObject {
    
    public class DataObject : GUIDraggableObject
    // This class just has the capability of being dragged in GUI - it could be any type of generic data class
    {
        private string name;
        private int value;

        public DataObject(string name, int value, Vector2 position) : base(position) {
            this.name = name;
            this.value = value;
        }

        public void draw(float scale) {
            Rect drawRect = new Rect(position.x, position.y, scale*200.0f, scale*100.0f);

            GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
            GUILayout.Label(name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

            Rect dragRect = GUILayoutUtility.GetLastRect();
            dragRect = new Rect(dragRect.x + position.x, dragRect.y + position.y, dragRect.width, dragRect.height);

            /*
            if (dragging) {
                GUILayout.Label("Wooo...");
            }
            else if (GUILayout.Button("Yes!")) {
                Debug.Log("Yes. It is " + value + "!");
            }
            */
            GUILayout.TextArea("");
            //GUILayout.Label("this is a very long text indeed. this is a very long text indeed. this is a very long text indeed. this is a very long text indeed. ");
            GUILayout.EndArea();

            dragHandler(dragRect);
        }
    }
}
