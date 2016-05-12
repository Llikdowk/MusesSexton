using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class PoemBehaviour : CharacterBehaviour {
        private readonly Vector3 originalCameraPos;
        private readonly Vector3 originalCameraRotation;
        private readonly Vector3 originalPoemCameraPos;
        private readonly Vector3 originalPoemCameraRotation;
        private readonly Camera poemCamera;

        public PoemBehaviour(GameObject character) : base(character) {
            originalCameraPos = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.eulerAngles;
            poemCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            originalPoemCameraPos = poemCamera.transform.position;
            originalPoemCameraRotation = poemCamera.transform.eulerAngles;
            poemCamera.enabled = true;
            Debug.Log("POEM");
            Cursor.visible = true;
        }

        public override void cinematicMode(bool enabled) {
            cinematic = enabled;
        }

        public override void destroy() {
            Camera.main.transform.position = originalCameraPos;
            Camera.main.transform.eulerAngles = originalCameraRotation;
            poemCamera.transform.position = originalPoemCameraPos;
            poemCamera.transform.eulerAngles = originalPoemCameraRotation;
            poemCamera.enabled = false;
            Cursor.visible = false;
        }

        private Vector3 p0, p1;
        public override void run() {
            if (cinematic) return;
            checkStateChange();
            //Vector3 mouse = Input.mousePosition;    
            Vector3 p = Input.mousePosition*2-Vector3.one;
            p0 = new Vector3(p.x/Screen.width, p.y/(2*Screen.height), 0);
            Vector3 dif = p0 - p1;
            //Camera.main.transform.localPosition += 2*dif;
            poemCamera.transform.localPosition += 2*dif;
            Camera.main.transform.Rotate(new Vector3(4*dif.y, 4*dif.x, 0), Space.Self); //5*dif;
            p1 = p0;
        }

        private void checkStateChange() {
            if (GameActions.checkAction(Action.DEBUG, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new WalkBehaviour(character);
            }
        }
    }
}
