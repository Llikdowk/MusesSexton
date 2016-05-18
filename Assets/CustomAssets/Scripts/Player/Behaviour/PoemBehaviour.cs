using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.Components;
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
        private TextSetComponent textSetComponent;
        private Ray ray;
        private RaycastHit hit;
        private const float maxDistance = 1000f;
        private bool textDisplayed = false;
        private readonly Stack<VerseTextComponent> lastTextColorChanged = new Stack<VerseTextComponent>(6);
        private bool textColored = false;
        private readonly Transform graveHollow;
        private const int mask = 1 << 10 | 1 << 8;
        private bool hasEnded = false;
        private SuperTestSet superTextSet = GameObject.Find("LandmarkSet").GetComponent<SuperTestSet>();
        private int currentVerseSelected = 0;

        private readonly TombstoneController tombstone;


        public PoemBehaviour(GameObject character, Transform graveHollow, GameObject tombstone) : base(character) {
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
            superTextSet.updateTextSetGenders();
            this.tombstone = tombstone.GetComponent<TombstoneController>();
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
            if (Physics.Raycast(ray, out hit, maxDistance, mask)) {
                Debug.DrawRay(Player.getInstance().eyeSight.position, hit.point - Player.getInstance().eyeSight.position, Color.magenta);

                if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    if (!textDisplayed && hit.collider.gameObject.tag == "landmark") {
                        GameObject textSet = hit.collider.gameObject.transform.parent.GetChild(0).gameObject;
                        textSetComponent = textSet.GetComponent<TextSetComponent>();
                        textSetComponent.doGoToPlayer(Player.getInstance().eyeSight);
                        Debug.Log("LANDMARK CLICKED");
                        textDisplayed = true;
                    }
                    else if (textDisplayed && hit.collider.gameObject.tag == "poemLetters") {
                        Debug.Log("TEXT SELECTED is " + hit.collider.gameObject.name);
                        GameObject aux = hit.collider.gameObject;
                        int n = (int)Char.GetNumericValue(aux.name[aux.name.Length - 1]);
                        textSetComponent.doGoToOrigin(n, graveHollow);
                        textSetComponent.updatePlayerState(n);
                        superTextSet.updateTextSetGenders();
                        textDisplayed = false;
                        //textTombstone[currentVerseSelected].text = textSetComponent.getTextOf(n);
                        tombstone.goUp(textSetComponent.getTextOf(n), currentVerseSelected);
                        ++currentVerseSelected;
                    }
                    else if (textDisplayed) {
                        textSetComponent.doGoToOrigin(-1, graveHollow);
                        textDisplayed = false;
                    }
                }

                if (textDisplayed && hit.collider.gameObject.tag == "poemLetters") {
                    VerseTextComponent t = hit.collider.gameObject.GetComponent<VerseTextComponent>();
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
                        textSetComponent.doGoToOrigin(-1, graveHollow);
                        textDisplayed = false;
                }
            }
        }

        private void cleanTextColor() {
            while (lastTextColorChanged.Count > 0) {
                VerseTextComponent t = lastTextColorChanged.Pop();
                t.setNormalColor();
            }
        }

        private void doMouseMovement() {
            mouseLook.LookRotation(character.transform, Camera.main.transform);
        }

        private void checkStateChange() {
            if (Player.getInstance().versesSelectedCount == 3) {
                Debug.LogWarning("CAMERA CHANGES TO BE DONE");

                currentVerseSelected = 0;
                Player.getInstance().versesSelectedCount = 0;
                Player.getInstance().genderChosen = Gender.UNDECIDED;
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
                hasEnded = true;
            }

            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
