
using Assets.CustomAssets.Scripts.Audio;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Components;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class CoffinDragBehaviour : CharacterBehaviour {
        private readonly FirstPersonController fps;
        private float t0 = 0f;
        private float t1 = 0f;
        private readonly float time_created = 0f;
        private const float fastTimeWindow_s = .5f;
        private const float slowTimeWindow_s = .35f;
        private const float force = 1f;
        private bool fast = false;
        private readonly GameObject coffin;
        private readonly Rigidbody coffinRb;
        private readonly MeshRenderer coffinMeshRenderer;
        private const float startDelay = .25f;
        private readonly Camera shovelCamera = GameObject.Find("3DUICamera").GetComponent<Camera>();


        public CoffinDragBehaviour(GameObject character, GameObject coffin) : base(character) {
            AudioUtils.controller.pickup_coffin.Play();
            time_created = Time.time;
            fps = Player.getInstance().gameObject.AddComponent<FirstPersonController>();
            configureController();
            Debug.Log("COFFIN MODE");
            this.coffin = coffin;
            coffin.transform.parent = Player.getInstance().coffinSlot;
            coffin.transform.localEulerAngles = Vector3.zero;
            coffin.transform.localPosition = Vector3.zero;
            coffinMeshRenderer = coffin.GetComponent<MeshRenderer>();
            coffinMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            this.coffinRb = coffin.GetComponent<Rigidbody>();
            coffinRb.isKinematic = true;
            shovelCamera.enabled = false;
        }

        private void configureController() {
            fastConfiguration();
            fps.m_GravityMultiplier = 1;
        }

        private void slowConfiguration() {
            fps.m_WalkSpeed = 2f;
            fps.m_RunSpeed = 2f;
        }

        private void fastConfiguration() {
            fps.m_WalkSpeed = 3f;
            fps.m_RunSpeed = 3f;
        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
            fps.enabled = !enabled;
        }

        public override void destroy() {
            shovelCamera.enabled = true;
            //coffinRb.isKinematic = false;
            coffin.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            UnityEngine.Object.Destroy(fps);
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            handleMovementSpeed();
        }

        private void handleMovementSpeed() {
            t0 = Time.time;
            if (!fast) {
                if (t0 - t1 > slowTimeWindow_s) {
                    t1 = Time.time;
                    fastConfiguration();
                    fast = true;
                }
            }
            else {
                if (t0 - t1 > fastTimeWindow_s) {
                    t1 = Time.time;
                    slowConfiguration();
                    fast = false;
                }
            }
        }


        private void checkStateChange() {
            if (!Player.getInstance().insideThrowCoffinTrigger && GameActions.checkAction(Action.USE, Input.GetKeyDown) && Time.time - time_created > startDelay) {
                Ray ray = new Ray(character.transform.position, character.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 2f)) {
                    Vector3 v = Vector3.ProjectOnPlane(character.transform.forward, Vector3.up);
                    float checker = Vector3.Dot(hit.normal, v);
                    if (checker < .35f && checker > -.35f) return;
                }

                coffin.transform.parent = null;
                coffinRb.isKinematic = false;
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
                AudioUtils.controller.throw_coffin.Play();
            }
        }
    }
}