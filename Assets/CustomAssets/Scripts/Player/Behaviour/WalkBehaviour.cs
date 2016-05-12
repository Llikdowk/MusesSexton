
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class WalkBehaviour : CharacterBehaviour {
        private readonly FirstPersonController fps;
        private static readonly GameObject coffinSet = GameObject.Find("CoffinSet");


        public WalkBehaviour(GameObject character) : base(character) {
            fps = character.AddComponent<FirstPersonController>();
            configureController();
            character.GetComponent<DigBehaviour>().enabled = true;
            Debug.Log("WALK MODE");
        }

        private void configureController() {
            fps.m_WalkSpeed = 4;
            fps.m_RunSpeed = 8;
            fps.m_JumpSpeed = 5;
            fps.m_GravityMultiplier = 1;
        }

        public override void cinematicMode(bool enabled) {
            cinematic = enabled;
            fps.enabled = !enabled;
        }

        public override void destroy() {
            UnityEngine.Object.Destroy(fps);
            character.GetComponent<DigBehaviour>().enabled = false;
        }

        public override void run() {
            if (cinematic) return;
            checkStateChange();
        }

        private void checkStateChange() {
            Player p = Player.getInstance();
            /*
            if (p.triggerCartBack && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                GameObject coffin = coffinSet.transform.GetChild(p.coffinsBuried).gameObject;
                p.behaviour = new CoffinDragBehaviour(character, coffin);
            } else */
            if (p.triggerCartFront && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                p.behaviour = new DriveCartBehaviour(character);
            }
        }
    }
}
