using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.Anmation;
using Assets.CustomAssets.Scripts.Audio;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public enum DigType { NORMAL, INVERSE };

    public class DigBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Vector3 originalCameraRotation;
        private readonly CursorLockMode cursorStateBackup;
        private readonly float time_created;
        private GameObject groundFloor;
        private GameObject heap;
        private GameObject tombstone;
        private const int limitHits = 3;
        private const float delay = .1f;
        private Vector3 p0, p1;
        private int hits = 0;
        private DigType type;

        public DigBehaviour(GameObject character, DigType type = DigType.NORMAL) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.eulerAngles;
            Debug.Log("DIG behaviour");
            Cursor.visible = true;
            cursorStateBackup = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            time_created = Time.time;
            this.type = type;
            AudioUtils.controller.enter_music_perc();
        }

        public void init(GameObject groundFloor, GameObject heap, GameObject tombstone) {
            this.groundFloor = groundFloor;
            this.heap = heap;
            this.tombstone = tombstone;
        }

        public override void destroy() {
            //Camera.main.transform.position = originalCameraPos;
            //Camera.main.transform.eulerAngles = originalCameraRotation;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
            AudioUtils.controller.exit_music_perc();
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();

            if (GameActions.checkAction(Action.USE, Input.GetKeyDown) && Time.time - time_created > delay) {
                if (type == DigType.NORMAL) {
                    AnimationUtils.launchDig(); // will launch digActionEvent
                    AudioUtils.controller.playDig();
                }
                else if (type == DigType.INVERSE) {
                    AnimationUtils.launchUndig();
                    AudioUtils.controller.playDig();
                }
            } else {
                doMouseMovement();
            }
        }
        

        public void launchActionEvent() {
            if (type == DigType.NORMAL) {
                AudioUtils.controller.playDig();
                groundFloor.transform.position -= Vector3.up * 1f;
                //groundFloor.transform.parent.GetChild(1).transform.localScale += Vector3.one * .25f;
                heap.transform.localScale += Vector3.up * .25f;
            } else {
                groundFloor.transform.position += Vector3.up * 1f;
                //groundFloor.transform.parent.GetChild(1).transform.localScale -= Vector3.one * .25f;
                heap.transform.localScale -= Vector3.up * .25f;
            }
        }

        public void launchEndActionEvent() {
            ++hits;
        }

        private void doMouseMovement() {
            Vector3 mouse = Input.mousePosition;
            p0 = new Vector3(mouse.x / Screen.width * 2 - 1, mouse.y / (2 * Screen.height) * 2 - 1, 0);
            Vector3 dif = p0 - p1;
            Camera.main.transform.Rotate(new Vector3(-5 * dif.y, 5 * dif.x, 0), Space.Self);
            p1 = p0;
        }

        private void checkStateChange() {
            if (hits == limitHits) {
                if (type == DigType.NORMAL) {
                    groundFloor.transform.parent.GetComponent<BoxCollider>().enabled = true;
                    Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
                } else if (type == DigType.INVERSE) {
                    groundFloor.transform.parent.GetComponent<BoxCollider>().enabled = false;
                    UIUtils.infoInteractive.text = "select verse!";
                    Player.getInstance().coffinBuriedAction();
                    Debug.LogWarning("MOVE CAMERA UPWARD!!!");
                    Player.getInstance().behaviour = new PoemBehaviour(character, tombstone);
                }
            }
        }
    }
}
