using Assets.CustomAssets.Scripts.DialogueSystem.Nodes;
using Assets.CustomAssets.Scripts.DialogueSystem.Render;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CustomAssets.Scripts.DialogueSystem {
    public class Dialogue : MonoBehaviour {
        public Text text;

        private DialogueManager dialogueMng;
        private DialogueNode iterator;
        private bool read = false;

        public void Start () {
            TextRender.UIText = text;
            dialogueMng = new DialogueManager();
            iterator = dialogueMng.generate();
        }

        public void Update() {
            if (!read && Input.GetMouseButtonDown(0)) {
                if (iterator != null) {
                    iterator = dialogueMng.runDialogue(iterator);
                    
                } else {
                    TextRender.clean();
                    read = true;
                }
            }
        }

        public void Answer1Selected() {
            iterator = selectAnswer(0);
            Debug.Log("question1 marked");
        }

        public void Answer2Selected() {
            iterator = selectAnswer(1);
            Debug.Log("question2 marked");
        }

        private DialogueNode selectAnswer(int n) {
            return iterator = dialogueMng.runDialogue(iterator.adjacent[n].next());
        }
    }
}
