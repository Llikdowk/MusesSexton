
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Audio {
    public static class AudioUtils {
        public static void digSound() {
            Debug.LogWarning("now playing: digging sound");
        }

        public static void undigSound() {
            Debug.LogWarning("now playing: undigging sound");
        }

        public static void playTombstoneShake() {
            Debug.LogWarning("now playing: tombstone shake sound");
        }

        public static void playTombstoneUp() {
            Debug.LogWarning("now playing: tombstone raising sound");
        }

        public static void throwCoffinInsideHollow() {
            Debug.LogWarning("now playing: throwing coffin inside hollow");
        }
    }
}
