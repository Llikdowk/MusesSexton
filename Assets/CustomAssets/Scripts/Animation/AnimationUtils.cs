
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Anmation {
    public class AnimationUtils : MonoBehaviour {
        public static readonly Animator cameraAnimator = GameObject.Find("AnimatorEntity").GetComponent<Animator>();
        private static readonly AnimationCurve throwCoffinCurve = GameObject.Find("ThrowCoffinCurve").GetComponent<trigger_hollow_behaviours>().curve;

        public static void launchDig() {
            cameraAnimator.Play("dig");
        }

        public static void launchUndig() {
            cameraAnimator.Play("undig");
        }

        public static AnimationCurve createStraightCurve() {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 1));
            curve.AddKey(new Keyframe(1, 1));
            return curve;
        }

        public static AnimationCurve createLinearCurve() {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, tan45, tan45));
            curve.AddKey(new Keyframe(1, 1, tan45, tan45));
            return curve;
        }

        public static AnimationCurve createEaseInEaseOutCurve() {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(1, 1, 0, 0));
            return curve;
        }

        public static AnimationCurve createThrowCoffinCurve() {
            return throwCoffinCurve;
        }
    }
}
