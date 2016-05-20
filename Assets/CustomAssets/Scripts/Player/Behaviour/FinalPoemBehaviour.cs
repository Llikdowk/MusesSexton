using System;
using System.Collections.Generic;
using Assets.CustomAssets.Scripts.Components;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using MouseLook = Assets.CustomAssets.Scripts.Components.MouseLook;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class FinalPoemBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Quaternion originalCameraRotation;
        private readonly CursorLockMode cursorStateBackup;
        private readonly MouseLook mouseLook;
        private TextSetComponent textSetComponent;
        private Ray ray;
        private RaycastHit hit;
        private const float maxDistance = 1000f;
        private readonly Stack<TextMesh> lastTextColorChanged = new Stack<TextMesh>(6);
        private bool textColored = false;
        private int currentVerseSelected = 0;
        private readonly AnimationCameraComponent cameraAnimationComponent;
        private readonly Camera shovelCamera = GameObject.Find("3DUICamera").GetComponent<Camera>();
        private readonly TombstoneController tombstone;
        private bool fovChanged = false;
        private bool versesDeployed = false;
        private readonly int originalMainCulling;
        private int mask = ~(1 << 9);
        public FinalPoemBehaviour(GameObject character, GameObject tombstone) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.rotation;
            originalMainCulling = Camera.main.cullingMask;
            Camera.main.cullingMask = Camera.main.cullingMask | 1<<8;
            Debug.Log("POEM");
            Cursor.visible = true;
            cursorStateBackup = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            mouseLook = new MouseLook();
            mouseLook.Init(character.transform, Camera.main.transform);
            mouseLook.XSensitivity = 1f;
            mouseLook.YSensitivity = 1f;
            mouseLook.smooth = true;
            this.tombstone = tombstone.GetComponent<TombstoneController>();
            this.cameraAnimationComponent = Player.getInstance().gameObject.transform.GetChild(0).GetComponent<AnimationCameraComponent>();
            shovelCamera.enabled = false;
            Player.getInstance().disableEyeSight();
        }


        public override void destroy() {
            shovelCamera.enabled = true;
            Camera.main.cullingMask = originalMainCulling;
            Cursor.lockState = cursorStateBackup;
            Cursor.visible = false;
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();
            doMouseMovement();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!versesDeployed) {
                deployTexts();
                return;
            }
            if (versesDeployed && Physics.Raycast(ray, out hit, maxDistance, mask)) {
                Debug.DrawRay(Player.getInstance().eyeSight.position, hit.point - Player.getInstance().eyeSight.position, Color.magenta);

                if (versesDeployed && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {

                    if (hit.collider.gameObject.tag == "poemLetters") {
                        GameObject textSet = hit.collider.gameObject.transform.parent.GetChild(0).gameObject;
                        textSetComponent = textSet.GetComponent<TextSetComponent>();

                        Debug.Log("TEXT SELECTED is " + hit.collider.gameObject.name);
                        GameObject aux = hit.collider.gameObject;
                        int n = (int)Char.GetNumericValue(aux.name[aux.name.Length - 1]);
                        if (n == 0)
                            tombstone.goUp(Player.getInstance().versesSelected[0+3*currentVerseSelected], currentVerseSelected);
                        else if (n == 2)
                            tombstone.goUp(Player.getInstance().versesSelected[1+3* currentVerseSelected], currentVerseSelected);
                        else if (n == 4)
                            tombstone.goUp(Player.getInstance().versesSelected[2+3* currentVerseSelected], currentVerseSelected);

                        ++currentVerseSelected;
                        Player.getInstance().versesSelectedCount++;
                        exitDisplayVerseMode();
                    } else {
                        //exitDisplayVerseMode();
                    }
                }

                if (hit.collider.gameObject.tag == "poemLetters") {
                    TextMesh t = hit.collider.gameObject.GetComponent<TextMesh>();
                    lastTextColorChanged.Push(t);
                    t.color = Player.getInstance().textOverColor;
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
                if (versesDeployed && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    exitDisplayVerseMode();
                }
                if (fovChanged) {
                    cameraAnimationComponent.setDefaultFov();
                    fovChanged = false;
                    textSetComponent.setNormalColor();
                }
            }
        }

        private void deployTexts() {
            if (Player.getInstance().versesSelectedCount == 3) { checkStateChange(); return;}
            Player.getInstance().unattachSight();
            Player.getInstance().setFinaleEyeSight();
            Debug.Log(Player.getInstance().versesSelected[0]);
            Player.getInstance().drawVerse(Player.getInstance().versesSelected[0+3* currentVerseSelected], 0);
            Player.getInstance().drawVerse(Player.getInstance().versesSelected[1+3* currentVerseSelected], 2);
            Player.getInstance().drawVerse(Player.getInstance().versesSelected[2+3* currentVerseSelected], 4);
            versesDeployed = true;
        }

        private void exitDisplayVerseMode() {
            Player.getInstance().cleanVerses();
            Player.getInstance().reatachSight();
            Player.getInstance().disableEyeSight();
            versesDeployed = false;
        }

        private void cleanTextColor() {
            while (lastTextColorChanged.Count > 0) {
                TextMesh t = lastTextColorChanged.Pop();
                t.color = Player.getInstance().textOriginalColor;
            }
        }

        private void doMouseMovement() {
            mouseLook.LookRotation(character.transform, Camera.main.transform);
        }

        private void checkStateChange() {
            if (Player.getInstance().versesSelectedCount == 3) {
                //Debug.LogWarning("END FINALE");
                //Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }

            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
