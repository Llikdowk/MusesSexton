using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CustomAssets.Scripts.DialogueSystem.Nodes {
    public class EmptyNode : DialogueNode {
        public override DialogueNode next() {
            DebugMsg.emptyNodeNext();
            return this;
        }

        public override void display() {
        }
    }
}
