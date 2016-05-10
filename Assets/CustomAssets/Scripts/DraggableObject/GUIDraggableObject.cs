using UnityEngine;

namespace Assets.CustomAssets.Scripts.DraggableObject {

    public class GUIDraggableObject {
        public Vector2 position;
        public bool dragging;
        public bool isFront = false;

        //private static Vector2 worldOffset = Vector2.zero;
        private bool middleDragging;

        private Vector2 dragStart;

        public GUIDraggableObject(Vector2 position) {
            this.position = position;
        }

        public void dragHandler(Rect draggingRect) {
            Event e = Event.current;
            
            //drag(draggingRect);
            if (e.type == EventType.MouseDrag && e.button == 0 && draggingRect.Contains(e.mousePosition)) {
                dragging = true;
                position += e.delta;
            }
            else if (e.type == EventType.MouseUp && e.button == 0 && dragging) {
                dragging = false;
            }


            if (EventUtils.mouseDown(1)) {
                Debug.Log("right mouse pressed");
            } else if (EventUtils.mouseUp(1)) {
                Debug.Log("right mouse released");
            }
        }
        
        private void drag(Rect draggingRect) {
            Event e = Event.current;
            if (EventUtils.mouseDown(0) && draggingRect.Contains(e.mousePosition)) {
                dragging = true;
                dragStart = e.mousePosition - position;
                e.Use();
            }
            else if (EventUtils.mouseUp(0)) {
                dragging = false;
            }
            if (dragging) {
                position = e.mousePosition - dragStart;
            }
        }

    }
}
