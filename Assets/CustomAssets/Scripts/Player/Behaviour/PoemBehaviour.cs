using System;
using System.Collections.Generic;
using Assets.CustomAssets.Scripts.Audio;
using Assets.CustomAssets.Scripts.Components;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using MouseLook = Assets.CustomAssets.Scripts.Components.MouseLook;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class PoemBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Quaternion originalCameraRotation;
        private readonly Vector3 originalPoemCameraPos;
        private readonly Quaternion originalPoemCameraRotation;
        //private readonly Camera poemCamera;
        private readonly CursorLockMode cursorStateBackup;
        private Vector3 p0, p1;
        private readonly MouseLook mouseLook;
        private TextSetComponent textSetComponent;
        private Ray ray;
        private RaycastHit hit;
        private const float maxDistance = 1000f;
        private bool textDisplayed = false;
        private readonly Stack<TextMesh> lastTextColorChanged = new Stack<TextMesh>(6);
        private bool textColored = false;
        private bool hasEnded = false;
        private int currentVerseSelected = 0;
        private readonly AnimationCameraComponent cameraAnimationComponent;
        private readonly Camera shovelCamera = GameObject.Find("3DUICamera").GetComponent<Camera>();
        private readonly TombstoneController tombstone;
        private bool fovChanged = false;
        private bool versesDeployed = false;
        private readonly int originalMainCulling;
        private int mask = ~(1 << 9);
        public PoemBehaviour(GameObject character, GameObject tombstone) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.rotation;
            //poemCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            //originalPoemCameraPos = poemCamera.transform.position;
            //originalPoemCameraRotation = poemCamera.transform.rotation;
            //poemCamera.enabled = true;
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
            //superTextSet.updateTextSetGenders();
            this.tombstone = tombstone.GetComponent<TombstoneController>();
            this.cameraAnimationComponent = Player.getInstance().gameObject.transform.GetChild(0).GetComponent<AnimationCameraComponent>();
            shovelCamera.enabled = false;
            Player.getInstance().disableEyeSight();
        }


        public override void destroy() {
            //Camera.main.transform.position = originalCameraPos;
            //Camera.main.transform.rotation = originalCameraRotation;
            //poemCamera.transform.position = originalPoemCameraPos;
            //poemCamera.transform.rotation = originalPoemCameraRotation;
            shovelCamera.enabled = true;
            //poemCamera.enabled = false;
            Camera.main.cullingMask = originalMainCulling;
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
                //Debug.Log("hit tag: " + hit.transform.tag);

                if (!versesDeployed && hit.collider.gameObject.tag == "landmark") {
                    UIUtils.landmarkFound();
                    /*
                    Vector3 v = hit.point;
                    Vector3 center = hit.transform.parent.position;
                    v = Vector3.ProjectOnPlane(v, Camera.main.transform.forward);
                    center = Vector3.ProjectOnPlane(center, Camera.main.transform.forward);
                    float distance = Vector3.Distance(v, center);
                    float x = 50f / (distance);
                    x = Mathf.Min(12.5f, x);
                    */
                    cameraAnimationComponent.setRelativeFov(-15);
                    fovChanged = true;

                    //Debug.Log("text set hit: " + hit.collider.gameObject.name + " of: " + hit.collider.gameObject.transform.parent.name);
                    GameObject textSet = hit.collider.gameObject.transform.parent.GetChild(0).gameObject;
                    textSetComponent = textSet.GetComponent<TextSetComponent>();
                    //float t = Mathf.Clamp(0, x / distance, 1);
                    textSetComponent.setOverColor(1.0f);

                    if (!versesDeployed && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                        AudioUtils.controller.playTone();
                        cameraAnimationComponent.applyShake(.75f, 10f, 0.7f);
                        cameraAnimationComponent.setDefaultFov();
                        float wait = 0.0f;
                        float waitStep = 0.15f;
                        Transform playerOrbSlot = Player.getInstance().orbSlotPosition;
                        Player.getInstance().unattachSight();
                        foreach (VerseTextComponent orb in textSetComponent.allOrbs) {
                            var orbAux = orb;
                            endAnimationCallback lambda =
                                () => {
                                    //cameraAnimationComponent.colorCorrection(2f);
                                    Transform text = Player.getInstance().drawVerse(orbAux.getVerse(), orbAux.index);
                                    Transform shadow = text.GetChild(0);
                                    textSetComponent.moveSubjectTo(text, text.position - character.transform.up * .25f, 0f);
                                    textSetComponent.moveSubjectTo(shadow, shadow.position - character.transform.up * .25f, 0f);
                                };
                            textSetComponent.moveSubjectTo(orb.transform, playerOrbSlot, wait, lambda);
                            wait += waitStep;
                            versesDeployed = true;
                            Player.getInstance().enableEyeSight();
                        }
                        return;
                    }
                } else {
                    UIUtils.forceClear();
                }


                if (versesDeployed && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {

                    if (hit.collider.gameObject.tag == "poemLetters") {
                        AudioUtils.controller.playTone();
                        AudioUtils.controller.crumbling.Play();
                        Debug.Log("TEXT SELECTED is " + hit.collider.gameObject.name);
                        GameObject aux = hit.collider.gameObject;
                        int n = (int)Char.GetNumericValue(aux.name[aux.name.Length - 1]);

                        //textSetComponent.doGoToOrigin(n, graveHollow);
                        textSetComponent.moveAllOrbsToOrigin();
                        textSetComponent.updatePlayerState(n);
                        //superTextSet.updateTextSetGenders();
                        textDisplayed = false;
                        //textTombstone[currentVerseSelected].text = textSetComponent.getTextOf(n);
                        Transform temp = new GameObject("temp").transform;
                        temp.position = character.transform.position;
                        temp.LookAt(tombstone.transform.position + Vector3.up);
                        cameraAnimationComponent.applyShake(.5f, .1f, 0.05f);
                        cameraAnimationComponent.moveTo(temp, 0.01f/*, () => { UnityEngine.Object.Destroy(temp.gameObject); }*/);
                        tombstone.goUp(textSetComponent.getTextOf(n), currentVerseSelected);
                        ++currentVerseSelected;

                        exitDisplayVerseMode();
                    }
                    else {
                        exitDisplayVerseMode();
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

                if (hit.collider.gameObject.tag != "poemLetters") {
                    if (fovChanged) {
                        cameraAnimationComponent.setDefaultFov();
                        fovChanged = false;
                        textSetComponent.setNormalColor();
                    }
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
                UIUtils.forceClear();
            }
        }

        private void exitDisplayVerseMode() {
            textSetComponent.moveAllOrbsToOrigin();
            Player.getInstance().cleanVerses();
            Player.getInstance().reatachSight();
            versesDeployed = false;
            Player.getInstance().disableEyeSight();
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
                Debug.LogWarning("CAMERA CHANGES TO BE DONE");
                AudioUtils.controller.bellDing();

                currentVerseSelected = 0;
                Player.getInstance().versesSelectedCount = 0;
                Player.getInstance().genderChosen = Gender.UNDECIDED;
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
                hasEnded = true;
                Player.getInstance().checkBuriedAllCoffins();
            }

            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
