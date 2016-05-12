
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player {
    public class Player {
        public bool triggerCartBack = false;
        public bool triggerCartFront = false;

        public int coffinsBuried = 0;

        public readonly GameObject gameObject;
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
            _behaviour = new WalkBehaviour(gameObject);
        }

        public CharacterBehaviour behaviour {
            get { return _behaviour; }
            set { _behaviour.destroy(); _behaviour = value; }
        }
    }
}
