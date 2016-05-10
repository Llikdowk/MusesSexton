using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Assets.CustomAssets.Scripts.DialogueSystem.Render;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Nodes {
    public class TextNode : DialogueNode {
        private readonly Random random = new Random();

        public TextNode(string text) : base(text) {
        }

        public override DialogueNode next() {
            if (adjacent.Count > 0) {
                return adjacent[random.Next(0, adjacent.Count)];
            }
            return null;
        }

        public override void display() {
            TextRender.displayText(text);
        }
    }
}
