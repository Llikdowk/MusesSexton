
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
            Debug.LogWarning("now playing: tombstone raising sound");
        }
    }
}
