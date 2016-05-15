using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class PoemBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Vector3 originalCameraRotation;
        private readonly Vector3 originalPoemCameraPos;
        private readonly Vector3 originalPoemCameraRotation;
        private readonly Camera poemCamera;
        private readonly CursorLockMode cursorStateBackup;
        private Vector3 p0, p1;
        private readonly MouseLook mouseLook;

        public PoemBehaviour(GameObject character) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.eulerAngles;
            poemCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            originalPoemCameraPos = poemCamera.transform.position;
            originalPoemCameraRotation = poemCamera.transform.eulerAngles;
            poemCamera.enabled = true;
            Debug.Log("POEM");
            Cursor.visible = true;
            cursorStateBackup = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            mouseLook = new MouseLook();
            mouseLook.Init(character.transform, Camera.main.transform);
            mouseLook.XSensitivity = 1f;
            mouseLook.YSensitivity = 1f;
            mouseLook.smooth = true;
        }


        public override void destroy() {
            Camera.main.transform.position = originalCameraPos;
            Camera.main.transform.eulerAngles = originalCameraRotation;
            poemCamera.transform.position = originalPoemCameraPos;
            poemCamera.transform.eulerAngles = originalPoemCameraRotation;
            poemCamera.enabled = false;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
        }

        private Ray ray;
        private RaycastHit hit;
        private float maxDistance = 1000;

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            doMouseMovement();
            if (Physics.Raycast(ray, out hit, maxDistance)) {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    if (hit.collider.gameObject.tag == "landmark") {
                        GameObject textSet = hit.collider.gameObject.transform.parent.GetChild(0).gameObject;
                        textSet.GetComponent<move_to_player>().doAction(Player.getInstance().eyeSight);
                        Debug.Log("LANDMARK CLICKED");
                    }
                }
                if (hit.collider.gameObject.tag == "poemLetters") {
                    hit.collider.gameObject.GetComponent<TextMesh>().color = Color.cyan;
                }
            }
        }

        private void doMouseMovement() {
            /*
            Vector3 mouse = Input.mousePosition;
            p0 = new Vector3(mouse.x / Screen.width * 2 - 1, mouse.y / (2 * Screen.height) * 2 - 1, 0);
            Vector3 dif = p0 - p1;
            Camera.main.transform.localPosition += new Vector3(dif.x / 2f, 0, 0);
            poemCamera.transform.localPosition += new Vector3(dif.x, 0, 0);
            Camera.main.transform.Rotate(new Vector3(-2 * dif.y, 2 * dif.x, 0), Space.Self);
            p1 = p0;
            */
            mouseLook.LookRotation(character.transform, Camera.main.transform);
        }

        private void checkStateChange() {
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
