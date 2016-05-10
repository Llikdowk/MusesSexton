using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.DialogueSystem.Render;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Nodes {
    public class QuestionNode : DialogueNode {
        private readonly EmptyNode answers;

        public QuestionNode(string text) : base(text) {
            answers = new EmptyNode() { adjacent = this.adjacent };
        }

        public override DialogueNode next() {
            return answers;
        }

        public override void display() {
            TextRender.displayQuestion(text);
            foreach (DialogueNode answer in adjacent) {
                answer.display();
            }
        }

        
    }
}
