
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class EmptyBehaviour : CharacterBehaviour {
        public override void cinematicMode(bool enabled) {
            cinematic = enabled;
        }

        public override void destroy() {
        }

        public override void run() {
        }

        public EmptyBehaviour(GameObject character) : base(character) {
        }
    }
}
