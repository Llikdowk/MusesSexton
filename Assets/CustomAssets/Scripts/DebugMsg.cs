using UnityEngine;

namespace Assets.CustomAssets.Scripts {
    public static class DebugMsg {
        public static void emptyNodeNext() {
            Debug.LogWarning("EmptyNode next() used");
        }

        public static void loadFail() {
            Debug.LogError("Cannot load object!");
        }
    }
}
