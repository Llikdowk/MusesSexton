using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class DriveCartBehaviour : CharacterBehaviour {
        public DriveCartBehaviour(GameObject character) : base(character) {
            Debug.Log("DRIVING!");
        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
        }

        public override void destroy() {
            //throw new NotImplementedException();
        }

        public override void run() {
            checkStateChange();
        }

        private void checkStateChange() {
            if(GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }
    }
}
