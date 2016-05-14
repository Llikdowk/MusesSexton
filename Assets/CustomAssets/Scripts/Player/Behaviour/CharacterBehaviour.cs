using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public abstract class CharacterBehaviour {
        public static bool cinematic { get; protected set; }
        protected GameObject character;

        protected CharacterBehaviour(GameObject character) {
            this.character = character;
        }

        public abstract void cinematicMode(bool enabled);
        public abstract void destroy();
        public abstract void run();
    }
}

