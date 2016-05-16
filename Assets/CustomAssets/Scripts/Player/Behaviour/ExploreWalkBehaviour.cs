
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class ExploreWalkBehaviour : CharacterBehaviour {
        private readonly FirstPersonController fps;
        private static readonly GameObject coffinSet = GameObject.Find("CoffinSet");
        private readonly float time_created = 0;
        private const float delay = 1f;

        public ExploreWalkBehaviour(GameObject character) : base(character) {
            fps = character.AddComponent<FirstPersonController>();
            configureController();
            RayExplorer db = character.GetComponent<RayExplorer>();
            db.enabled = true;
            time_created = Time.time;
            Debug.Log("WALK MODE -> digBehaviour set to " + db.enabled);
        }

        private void configureController() {
            fps.m_WalkSpeed = 4;
            fps.m_RunSpeed = 8;
            fps.m_JumpSpeed = 5;
            fps.m_GravityMultiplier = 1;
        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
            fps.enabled = !enabled;
        }

        public override void destroy() {
            UnityEngine.Object.Destroy(fps);
            character.GetComponent<RayExplorer>().enabled = false;
        }

        public override void run() {
            checkStateChange();
        }

        private void checkStateChange() {
            if (Time.time - time_created < delay) return;
            Player p = Player.getInstance();
            
            if (p.insideCartDrive && GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                p.behaviour = new DriveCartBehaviour(character);
            }
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                p.behaviour = new PoemBehaviour(character, Player.getInstance().gameObject.transform);
            }
        }
    }
}
