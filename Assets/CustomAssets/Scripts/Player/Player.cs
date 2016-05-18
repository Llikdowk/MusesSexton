
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

        public void addVerse(string value) {
            versesSelected[versesSelectedNext] = value;
            Debug.Log("first person verse: " + value);
        }

        public void coffinBuriedAction() {
            ++coffinsBuried;
        }

        public readonly GameObject gameObject;
        public readonly Transform coffinSlot;
        public readonly Transform eyeSight;
        private static Player instance;
        private CharacterBehaviour _behaviour;

        public static Player getInstance() {
            if (instance == null) {
                instance = new Player();
            }
            return instance;
        }

        private Player() {
            gameObject = GameObject.Find("Player");
            eyeSight = GameObject.Find("EyeSight").transform;
            coffinSlot = GameObject.Find("CoffinSlot").transform;
            _behaviour = new EmptyBehaviour(gameObject);
        }

        public CharacterBehaviour behaviour {
            get { return _behaviour; }
            set { _behaviour.destroy(); _behaviour = value; }
        }

        public bool cinematic {
            get { return CharacterBehaviour.cinematic; }
            set { _behaviour.cinematicMode(value); }
        }

        public void doMovementDisplacement() {
            
        }
    }
}
