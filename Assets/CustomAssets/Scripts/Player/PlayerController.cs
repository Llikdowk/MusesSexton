
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        private Player player;
        public bool cinematicMode;

        public void Start() {
            player = Player.getInstance();
        }

        public void Update() {
            player.behaviour.run();
            /*
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Debug.Log("CINEMATIC MODE TRUE");
                player.behaviour.cinematicMode(true);
            }
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyUp)) {
                Debug.Log("CINEMATIC MODE FALSE");
                player.behaviour.cinematicMode(false);
            }
            */
        }
    }
}
