using System;
using System.Collections.Generic;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Nodes {
    public abstract class DialogueNode {
        public List<DialogueNode> adjacent;
        public string text;

        protected DialogueNode(string text) {
            this.adjacent = new List<DialogueNode>();
            this.text = text;
        }

        public void addChild(DialogueNode child) {
            adjacent.Add(child);
        }

        public void removeChild(DialogueNode child) {
            adjacent.Remove(child);
        }

        protected DialogueNode() {
            adjacent = new List<DialogueNode>();
            text = "";
        }

        public abstract DialogueNode next();
        public abstract void display();
    }
}
