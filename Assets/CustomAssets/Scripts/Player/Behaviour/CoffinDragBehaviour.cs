
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.Characters.FirstPerson;

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

        public CoffinDragBehaviour(GameObject character, GameObject coffin) : base(character) {
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
        }

        private void configureController() {
            fps.m_WalkSpeed = .5f;
            fps.m_RunSpeed = .5f;
            fps.m_JumpSpeed = 0;
            fps.m_GravityMultiplier = 1;
        }

        private void slowConfiguration() {
            fps.m_WalkSpeed = .5f;
            fps.m_RunSpeed = .75f;
        }

        private void fastConfiguration() {
            fps.m_WalkSpeed = 2f;
            fps.m_RunSpeed = 2.5f;
        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
            fps.enabled = !enabled;
        }

        public override void destroy() {
            coffinRb.isKinematic = false;
            coffin.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            UnityEngine.Object.Destroy(fps);
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            handleMovementSpeed();

            /*
            Transform playerTransform = Player.getInstance().gameObject.transform;
            coffinRb.AddForce((playerTransform.position + Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up)*2 - coffin.transform.position).normalized
                * force * coffinRb.mass, ForceMode.Impulse);
                */
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
            if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && Time.time - time_created > startDelay) {
                coffin.transform.parent = null;
                coffinRb.isKinematic = false;
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}