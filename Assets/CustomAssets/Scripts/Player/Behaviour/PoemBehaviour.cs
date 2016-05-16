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
        private readonly Quaternion originalCameraRotation;
        private readonly Vector3 originalPoemCameraPos;
        private readonly Quaternion originalPoemCameraRotation;
        private readonly Camera poemCamera;
        private readonly CursorLockMode cursorStateBackup;
        private Vector3 p0, p1;
        private readonly MouseLook mouseLook;
        private move_to_player textMovement;
        private Ray ray;
        private RaycastHit hit;
        private float maxDistance = 1000;
        private bool textDisplayed = false;
        private Stack<verse_text> lastTextColorChanged = new Stack<verse_text>(6);
        private bool textColored = false;
        private Transform graveHollow;

        public PoemBehaviour(GameObject character, Transform graveHollow) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.rotation;
            poemCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            originalPoemCameraPos = poemCamera.transform.position;
            originalPoemCameraRotation = poemCamera.transform.rotation;
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
            this.graveHollow = graveHollow;
        }


        public override void destroy() {
            //Camera.main.transform.position = originalCameraPos;
            //Camera.main.transform.rotation = originalCameraRotation;
            //poemCamera.transform.position = originalPoemCameraPos;
            //poemCamera.transform.rotation = originalPoemCameraRotation;
            poemCamera.enabled = false;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
        }

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
                        textMovement = textSet.GetComponent<move_to_player>();
                        textMovement.doGoToPlayer(Player.getInstance().eyeSight);
                        Debug.Log("LANDMARK CLICKED");
                        textDisplayed = true;
                    }
                    else if (textDisplayed && hit.collider.gameObject.tag == "poemLetters") {
                        Debug.Log("TEXT SELECTED is " + hit.collider.gameObject.name);
                        GameObject aux = hit.collider.gameObject;
                        int n = (int)Char.GetNumericValue(aux.name[aux.name.Length - 1]);
                        textMovement.doGoToOrigin(n, graveHollow);
                        textDisplayed = false;
                    }
                    else if (textDisplayed) {
                        textMovement.doGoToOrigin(-1, graveHollow);
                        textDisplayed = false;
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
                if (textDisplayed && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                        textMovement.doGoToOrigin(-1, graveHollow);
                        textDisplayed = false;
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
