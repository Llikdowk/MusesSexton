
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using Cubiquity;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        private Player player;

        public void Start() {
            player = Player.getInstance();
            player.behaviour = new ExploreWalkBehaviour(player.gameObject);
        }

        public void OnEnabled() {
            player.behaviour = new ExploreWalkBehaviour(player.gameObject);
        }

        public void Update() {
            player.behaviour.run();
        }

        public void LateUpdate() {
            UIUtils.checkForClearAction();
        }
    }
}
