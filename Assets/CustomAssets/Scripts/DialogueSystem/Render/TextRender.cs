using Assets.CustomAssets.Scripts.DialogueSystem.Nodes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Render {
    public static class TextRender {
        public static Text UIText;
        public static GameObject[] UIButtons;

        public static void displayText(string text) {
            UIText.text = text;
        }

        public static void displayQuestion(string question) {
            for (int i = 0; i < UIButtons.Length; ++i) {
                UIButtons[i].SetActive(true);
            }
            UIText.text = "   QUESTION: " + question + "\n";
        }
        public static void displayAnswer(string answer, AnswerNode node) {
            UIText.text += ">> ANSWER: " + answer + "\n";
        }

        public static void clean() {
            UIText.text = "";
        }
    }
}
