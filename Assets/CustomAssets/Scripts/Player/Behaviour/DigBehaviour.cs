using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class DigBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Vector3 originalCameraRotation;
        private readonly CursorLockMode cursorStateBackup;
        private int hits = 0;
        private const int limitHits = 3;
        private float time_created;
        private const float delay = .25f;

        public DigBehaviour(GameObject character) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.eulerAngles;
            Debug.Log("DIG");
            Cursor.visible = true;
            cursorStateBackup = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            time_created = Time.time;
        }


        public override void destroy() {
            Camera.main.transform.position = originalCameraPos;
            Camera.main.transform.eulerAngles = originalCameraRotation;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
        }

        private Vector3 p0, p1;
        public override void run() {
            if (cinematic) return;
            checkStateChange();
            doMouseMovement();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10)) {
                GameObject impacted = hit.collider.gameObject;
                if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && Time.time - time_created > delay) {
                    if (impacted.tag == "groundGrave") {
                        impacted.transform.position -= Vector3.up * .25f;
                        impacted.transform.parent.GetChild(1).transform.localScale += Vector3.one * .25f;
                        ++hits;
                    }
                }
            }
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
