using Assets.CustomAssets.Scripts.Audio;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Assets.CustomAssets.Scripts.Components {

    public delegate void endAnimationCallback();

    public class AnimationCameraComponent : MonoBehaviour {
        private Transform player;
        //private Transform cameraMain;
        //private Transform camera3d;

        private Camera cameraMain;
        private Camera camera3d;
        //private Camera cameraPoem;

        private float defaultMainFov;
        private float defaultPoemFov;

        private DepthOfField dop;
        private ColorCorrectionCurves cc;
        
        internal void Start () {
            player = transform.parent;
            cameraMain = Camera.main;
            camera3d = GameObject.Find("3DUICamera").GetComponent<Camera>();
            //cameraPoem = GameObject.Find("Poem Camera").GetComponent<Camera>();

            defaultMainFov = cameraMain.fieldOfView;
            //defaultPoemFov = cameraPoem.fieldOfView;
            dop = Camera.main.GetComponent<DepthOfField>();
            cc = Camera.main.GetComponent<ColorCorrectionCurves>();

        }
	
        public void setFov(float fov) {
            cameraMain.fieldOfView = fov;
            //cameraPoem.fieldOfView = fov - 5;
        }

        public void setDefaultFov() {
            StartCoroutine(resetFov());
            //cameraMain.fieldOfView = defaultMainFov;
            //cameraPoem.fieldOfView = defaultPoemFov;
        }

        private IEnumerator resetFov() {
            float mainFov = cameraMain.fieldOfView;
            //float poemFov = cameraPoem.fieldOfView;
            float t = 0.0f;
            while (t < 1.0f) {
                cameraMain.fieldOfView = Mathf.Lerp(mainFov, defaultMainFov, t);
                //cameraPoem.fieldOfView = Mathf.Lerp(poemFov, defaultPoemFov, t);
                t += 0.1f;
                yield return new WaitForFixedUpdate();
            }
        }

        public void setRelativeFov(float offsetfov) {
            cameraMain.fieldOfView = defaultMainFov + offsetfov;
            //cameraPoem.fieldOfView = defaultPoemFov + offsetfov;
        }


        public void activateDOF() {
            dop.enabled = true;
        }
        public void deactivateDOF() {
            dop.enabled = false;
        }

        public void colorCorrection(float t) {
            cc.saturation = t;
        }

        private IEnumerator doColorCorrection(float t) {
            while (t > 1f) {
                cc.saturation = t;
                t -= 0.05f;
                yield return new WaitForEndOfFrame();
            }
        }

        public void applyShake(float shake, float decreaseFactor = 10.0f) {
            StartCoroutine(doShake(shake, decreaseFactor));
            AudioUtils.playTombstoneShake();
        }

        private IEnumerator doShake(float shake, float decreaseFactor) {
            Vector3 originalPos = cameraMain.transform.localPosition;
            float shakeAmount = 0.7f;
            while (shake > 0) {
                cameraMain.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                shake -= Time.deltaTime * decreaseFactor;
                yield return new WaitForFixedUpdate();
            }
            shake = 0;
            cameraMain.transform.localPosition = originalPos;
        }

        public void moveTo(Transform destination, params endAnimationCallback[] f) {
            Player.Player.getInstance().cinematic = true;
            StartCoroutine(doMoveTo(destination, f));
        }

        private IEnumerator doMoveTo(Transform destination, params endAnimationCallback[] f) {
            Transform origin = player.transform;
            float t = 0.0f;
            while (t < 1.0f) {
                t += 0.032f;
                player.position = Vector3.Slerp(origin.position, new Vector3(destination.position.x, player.position.y, destination.position.z), t);
                //player.eulerAngles = Vector3.Slerp(origin.eulerAngles, destination.eulerAngles, t);
                player.rotation = Quaternion.Slerp(origin.rotation, destination.rotation, t);
                yield return new WaitForFixedUpdate();
            }

            foreach (var x in f) x();
            Player.Player.getInstance().cinematic = false;
        }
    }
}
