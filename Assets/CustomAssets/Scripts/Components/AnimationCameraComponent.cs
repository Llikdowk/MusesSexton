using System.Collections;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Components {

    public delegate void endAnimationCallback();

    public class AnimationCameraComponent : MonoBehaviour {
        private Transform player;
        private Transform cameraMain;
        private Transform camera3d;
        
        internal void Start () {
            player = transform.parent;
            cameraMain = Camera.main.transform;
            camera3d = GameObject.Find("3DUICamera").transform;
        }
	
        public void moveTo(Transform destination, params endAnimationCallback[] f) {
            Player.Player.getInstance().cinematic = true;
            StartCoroutine(doMoveTo(destination, f));
        }

        private IEnumerator doMoveTo(Transform destination, params endAnimationCallback[] f) {
            Transform origin = player.transform;
            float t = 0.0f;
            while (t < 1.0f) {
                t += 0.05f;
                player.position = Vector3.Slerp(origin.position, new Vector3(destination.position.x, player.position.y, destination.position.z), t);
                //player.eulerAngles = Vector3.Slerp(origin.eulerAngles, destination.eulerAngles, t);
                player.rotation = Quaternion.Slerp(origin.rotation, destination.rotation, t);
                yield return new WaitForSeconds(0.016f);
            }

            foreach (var x in f) x();
            Player.Player.getInstance().cinematic = false;
        }
    }
}
