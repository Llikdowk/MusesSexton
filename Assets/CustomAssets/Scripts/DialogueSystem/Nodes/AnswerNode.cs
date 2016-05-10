
using Assets.CustomAssets.Scripts.DialogueSystem.Render;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Nodes {
    public class AnswerNode : TextNode {
        public AnswerNode(string text) : base(text) {
        }

        public override void display() {
            TextRender.displayAnswer(text, this);
        }
    }
}
