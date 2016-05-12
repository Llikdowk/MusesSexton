using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class CoffinDragBehaviour : CharacterBehaviour {
        private readonly FirstPersonController fps;
        private float t0 = 0f;
        private float t1 = 0f;
        private const float fastTimeWindow_s = .5f;
        private const float slowTimeWindow_s = .35f;
        private const float force = 1f;
        private bool fast = false;
        private GameObject coffin;
        private Rigidbody coffinRb;

        public CoffinDragBehaviour(GameObject character, GameObject coffin) : base(character) {
            fps = Player.getInstance().gameObject.AddComponent<FirstPersonController>();
            configureController();
            Debug.Log("COFFIN MODE");
            this.coffin = coffin;
            this.coffinRb = coffin.GetComponent<Rigidbody>();
            coffinRb.isKinematic = false;
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
            fps.enabled = !enabled;
        }

        public override void destroy() {
            UnityEngine.Object.Destroy(fps);
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            handleMovementSpeed();

            Transform playerTransform = Player.getInstance().gameObject.transform;
            coffinRb.AddForce((playerTransform.position + Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up)*2 - coffin.transform.position).normalized
                * force * coffinRb.mass, ForceMode.Impulse);
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
            Player p = Player.getInstance();
            if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                coffinRb.isKinematic = true;
                Player.getInstance().behaviour = new WalkBehaviour(character);
            }
        }
    }
}