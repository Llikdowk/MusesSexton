
using Assets.CustomAssets.Scripts.Anmation;
using Assets.CustomAssets.Scripts.Audio;
using Assets.CustomAssets.Scripts.Components;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player {
    public class Player {
        public bool insideCartDrive = false;
        public bool digNewHolesDisabled = false;
        public bool insideThrowCoffinTrigger = false;
        private int coffinsBuried = 0;
        public Gender genderChosen = Gender.UNDECIDED;
        public int versesSelectedCount = 0;
        public string[] versesSelected = new string[3 * 3];
        private int versesSelectedNext = 0;
        public Color textOriginalColor { get; private set; }
        public Color textOverColor { get; private set; }
        public Color shadowColor = Color.black;

        public readonly GameObject gameObject;
        public readonly Transform coffinSlot;
        public readonly Transform eyeSight;
        public readonly Transform eyeSightParent;
        public readonly Transform orbSlotPosition;
        private static Player instance;
        private CharacterBehaviour _behaviour;
        public readonly AnimationCameraComponent cameraAnimation = GameObject.Find("AnimatorEntity").GetComponent<AnimationCameraComponent>();
        private readonly TextMesh[] verses = new TextMesh[6];
        private readonly Vector3[] originalVersesLocalPos = new Vector3[6];
        private readonly TextMesh[] shadowVerses = new TextMesh[6];

        private open_gates giantDoorControl;

        //private DigBehaviour digBehaviourSaved;

        public void addVerse(string value) {
            versesSelected[versesSelectedNext++] = value;
            Debug.Log("first person verse: " + value);
        }

        public void coffinBuriedAction() {
            ++coffinsBuried;
            Debug.Log("coffins buried: " + coffinsBuried);
            switch(coffinsBuried) {
                case 1: AudioUtils.controller.enter_music_1(); break;
                case 2: AudioUtils.controller.enter_music_2(); break;
                case 3: AudioUtils.controller.enter_music_3(); break;
            }
        }

        public static Player getInstance() {
            if (instance == null) {
                instance = new Player();
            }
            return instance;
        }

        private Player() {
            gameObject = GameObject.Find("Player");
            eyeSight = GameObject.Find("EyeSight").transform;
            eyeSightParent = eyeSight.transform.parent;
            coffinSlot = GameObject.Find("CoffinSlot").transform;
            orbSlotPosition = GameObject.Find("OrbPosition").transform;
            giantDoorControl = GameObject.Find("GatesAsset").GetComponent<open_gates>();
            _behaviour = new EmptyBehaviour(gameObject);

            for (int i = 0; i < verses.Length; ++i) {
                verses[i] = eyeSight.GetChild(i).GetComponent<TextMesh>();
                originalVersesLocalPos[i] = verses[i].transform.localPosition;

                GameObject shadow = new GameObject("shadow");
                shadow.transform.parent = verses[i].transform;
                shadow.transform.localPosition = new Vector3(0, 0, 1.0f);
                shadow.transform.localEulerAngles = Vector3.zero;
                shadow.transform.localScale = Vector3.one;
                shadow.layer = verses[i].transform.gameObject.layer;
                TextMesh shadowText = shadow.AddComponent<TextMesh>();
                shadowText.font = verses[i].font;
                shadowText.fontSize = verses[i].fontSize;
                shadowText.alignment = verses[i].alignment;
                shadowText.anchor = verses[i].anchor;
                shadowText.color = shadowColor;
                shadowVerses[i] = shadowText;
            }
            textOriginalColor = verses[0].color;
            textOverColor = new Color(122f/256f, 0, 0);

            versesSelected[0] = "A";
            versesSelected[1] = "B";
            versesSelected[2] = "C";
            versesSelected[3] = "D";
            versesSelected[4] = "E";
            versesSelected[5] = "F";
            versesSelected[6] = "G";
            versesSelected[7] = "H";
            versesSelected[8] = "I";
        }

        public CharacterBehaviour behaviour {
            get { return _behaviour; }
            set { _behaviour.destroy(); _behaviour = value; }
        }

        public bool cinematic {
            get { return CharacterBehaviour.cinematic; }
            set { _behaviour.cinematicMode(value); }
        }

        public void checkBuriedAllCoffins() {
            if (coffinsBuried == 3) {
                AudioUtils.giantDoorOpeningSound();
                cameraAnimation.applyShake(10.0f, 2.5f, 0.7f);
                giantDoorControl.active = true;
            }
        }

        public void enableEyeSight() {
            eyeSight.gameObject.SetActive(true);
        }

        public void setFinaleEyeSight() {
            eyeSight.gameObject.SetActive(true);
            eyeSight.GetChild(1).gameObject.SetActive(false);
            eyeSight.GetChild(3).gameObject.SetActive(false);
            eyeSight.GetChild(5).gameObject.SetActive(false);
        }

        private void resetEyeSightTransform() {
            for (int i = 0; i < verses.Length; ++i) {
                verses[i].transform.localPosition = originalVersesLocalPos[i];
            }
        }

        public void offsetEyeSight(Vector3 offset) {
            for (int i = 0; i < verses.Length; ++i) {
                verses[i].transform.localPosition += offset;
            }
        }

        public void disableEyeSight() {
            resetEyeSightTransform();
            eyeSight.gameObject.SetActive(false);
        }

        public void doMovementDisplacement(Transform destination, float velocity, params endAnimationCallback[] f) {
            cameraAnimation.moveTo(destination, velocity, f);
        }

        public void unattachSight(Transform newParent = null) {
            eyeSight.transform.parent = null;
        }

        public void reatachSight() {
            eyeSight.transform.parent = eyeSightParent;
        }

        public Transform drawVerse(string verse, int slotPosition) {
            verses[slotPosition].text = verse;
            shadowVerses[slotPosition].text = verse;
            return verses[slotPosition].transform;
        }

        public void cleanVerses() {
            Debug.LogWarning("TO CLEAN");
            for (int i = 0; i < verses.Length; ++i) {
                verses[i].text = "";
                shadowVerses[i].text = "";
            }

            /*
            public void setDigBehaviour(DigBehaviour d) {
                digBehaviourSaved = d;
            }

            public void applyDigBehaviour() {
                behaviour = digBehaviourSaved;
            }
            */
        }
    }
}
