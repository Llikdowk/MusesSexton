
using System.Collections;
using System.Collections.Generic;
using Assets.CustomAssets.Scripts.DialogueSystem.Nodes;

namespace Assets.CustomAssets.Scripts.DialogueSystem {
    public class DialogueManager {

        public DialogueNode generate() {
            DialogueNode root = new TextNode("text1");
            DialogueNode question1 = new QuestionNode("question1");
            DialogueNode answer1 = new AnswerNode("answer1");
            answer1.addChild(new TextNode("answer1 ANSWERED!"));
            DialogueNode answer2 = new AnswerNode("answer1");
            answer2.addChild(root);
            question1.addChild(answer1);
            question1.addChild(answer2);
            root.addChild(question1);

            return root;
        }

        public DialogueNode runDialogue(DialogueNode node) {
            node.display();
            return node.next();
        }
    }
}
