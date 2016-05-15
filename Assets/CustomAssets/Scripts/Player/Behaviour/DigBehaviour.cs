using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.Anmation;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class DigBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Vector3 originalCameraRotation;
        private readonly CursorLockMode cursorStateBackup;
        private readonly float time_created;
        private readonly GameObject impacted;
        private const int limitHits = 3;
        private const float delay = .25f;
        private Vector3 p0, p1;
        private int hits = 0;

        public DigBehaviour(GameObject character, GameObject impacted) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.eulerAngles;
            Debug.Log("DIG behaviour");
            Cursor.visible = true;
            cursorStateBackup = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            time_created = Time.time;
            this.impacted = impacted;
        }


        public override void destroy() {
            Camera.main.transform.position = originalCameraPos;
            Camera.main.transform.eulerAngles = originalCameraRotation;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();

            if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && Time.time - time_created > delay) {
                AnimationUtils.launchDig(); // will launch digActionEvent
            } else {
                doMouseMovement();
            }
        }
        

        public void launchActionEvent() {
            impacted.transform.position -= Vector3.up * 1f;
            impacted.transform.parent.GetChild(1).transform.localScale += Vector3.one * .25f;
        }

        public void launchEndActionEvent() {
            ++hits;
            BoxCollider[] boxes = impacted.transform.parent.GetComponents<BoxCollider>();
            BoxCollider trigger = null;
            foreach(BoxCollider b in boxes) {
                if (b.isTrigger) {
                    trigger = b;
                }
            }
            trigger.enabled = true;
            impacted.tag = "Untagged";
        }

        private void doMouseMovement() {
            Vector3 mouse = Input.mousePosition;
            p0 = new Vector3(mouse.x / Screen.width * 2 - 1, mouse.y / (2 * Screen.height) * 2 - 1, 0);
            Vector3 dif = p0 - p1;
            //Camera.main.transform.localPosition += new Vector3(dif.x / 2f, 0, 0);
            Camera.main.transform.Rotate(new Vector3(-5 * dif.y, 5 * dif.x, 0), Space.Self);
            p1 = p0;
        }

        private void checkStateChange() {
            if (hits == limitHits) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
