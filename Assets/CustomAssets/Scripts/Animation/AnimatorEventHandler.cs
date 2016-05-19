using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Animation {
    public class AnimatorEventHandler : MonoBehaviour {

        public void enterCinematicMode() {
            Player.Player.getInstance().cinematic = true;
        }

        public void exitCinematicMode() {
            Player.Player.getInstance().cinematic = false;
        }

        public void digAction() {
            DigBehaviour dig = (DigBehaviour)Player.Player.getInstance().behaviour;
            dig.launchActionEvent();
        }

        public void digEndAction() {
            DigBehaviour dig = (DigBehaviour)Player.Player.getInstance().behaviour;
            dig.launchEndActionEvent();
        }
    }
}
