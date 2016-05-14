
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Anmation {
    public class AnimationUtils : MonoBehaviour {
        public static Animator cameraAnimator = GameObject.Find("AnimatorEntity").GetComponent<Animator>();

        public static void launchDig() {
            cameraAnimator.Play("dig");
        }
        
    }
}
