
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Audio {
    public static class AudioUtils {
        public static AudioController controller = GameObject.Find("AudioSystem").GetComponent<AudioController>();

        public static void digSound() {
            Debug.LogWarning("now playing: digging sound");
            //AudioUtils.controller.playDig();
        }

        public static void undigSound() {
            Debug.LogWarning("now playing: undigging sound");
            //AudioUtils.controller.playDig();
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

        public static void giantDoorOpeningSound() {
            AudioUtils.controller.ending_door.Play();
            Debug.LogWarning("now playing: giant door opening sound");
        }
    }
}
