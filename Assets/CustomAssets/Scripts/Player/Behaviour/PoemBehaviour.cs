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
        private bool textDisplayed = false;

        private Stack<verse_text> lastTextColorChanged = new Stack<verse_text>(6);
        private bool textColored = false;

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            doMouseMovement();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, maxDistance)) {
                Debug.DrawRay(Player.getInstance().eyeSight.position, hit.point - Player.getInstance().eyeSight.position, Color.magenta);

                if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    if (!textDisplayed && hit.collider.gameObject.tag == "landmark") {
                        GameObject textSet = hit.collider.gameObject.transform.parent.GetChild(0).gameObject;
                        textSet.GetComponent<move_to_player>().doAction(Player.getInstance().eyeSight);
                        Debug.Log("LANDMARK CLICKED");
                        textDisplayed = true;
                    }
                    else if (textDisplayed && hit.collider.gameObject.tag == "poemLetters") {
                        Debug.Log("TEXT SELECTED is " + hit.collider.gameObject.name);
                    }
                }

                if (textDisplayed && hit.collider.gameObject.tag == "poemLetters") {
                    verse_text t = hit.collider.gameObject.GetComponent<verse_text>();
                    lastTextColorChanged.Push(t);
                    t.setOverColor();
                    textColored = true;
                }
                else if (textColored) {
                    cleanTextColor();
                }
            }
            else {
                if (lastTextColorChanged.Count > 0) {
                    cleanTextColor();
                }
            }
        }

        private void cleanTextColor() {
            while (lastTextColorChanged.Count > 0) {
                verse_text t = lastTextColorChanged.Pop();
                t.setNormalColor();
            }
        }

        private void doMouseMovement() {
            mouseLook.LookRotation(character.transform, Camera.main.transform);
        }

        private void checkStateChange() {
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
