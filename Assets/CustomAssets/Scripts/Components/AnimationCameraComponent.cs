using System.Collections;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Components {
    public class AnimationCameraComponent : MonoBehaviour {
        private Transform player;
        
        internal void Start () {
            player = transform.parent;
        }
	
        public void moveTo(Transform destination) {
            StartCoroutine(doMoveTo(destination));
        }

        private IEnumerator doMoveTo(Transform destination) {
            Transform origin = player.transform;
            float t = 0.0f;
            while (t < 1.0f) {
                t += 0.05f;
                player.position = Vector3.Slerp(origin.position, new Vector3(destination.position.x, player.position.y, destination.position.z), t);
                //player.rotation = Quaternion.Slerp(origin.rotation, destination.rotation, t);
                yield return new WaitForSeconds(0.016f);
            }
        }
    }
}
