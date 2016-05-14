
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        private Player player;

        public void Start() {
            player = Player.getInstance();
        }

        public void Update() {
            player.behaviour.run();
        }
    }
}
