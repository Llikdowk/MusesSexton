using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.DraggableObject {
    public static class EventUtils {
        public static bool mouseUp(int n) {
            return Event.current.type == EventType.MouseUp && Event.current.button == n;
        }

        public static bool mouseDown(int n) {
            return Event.current.type == EventType.MouseDown && Event.current.button == n;
        }

        public static bool mouseWheelUp() {
            return Event.current.type == EventType.ScrollWheel && Event.current.delta.y < 0;
        }

        public static bool mouseWheelDown() {
            return Event.current.type == EventType.ScrollWheel && Event.current.delta.y > 0;
        }
    }
}
