using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.CustomInput {
    public class GameActions {
        private static GameActions instance;
        private static List<KeyCode> iterable; // can this be problematic with MT?

        public readonly Dictionary<Action, List<KeyCode>> keymap = new Dictionary<Action, List<KeyCode>>();
        public delegate bool inputCheckType(KeyCode k);

        private void registerAction(Action action, KeyCode key) {
            if (keymap.ContainsKey(action)) {
                List<KeyCode> list = keymap[action];
                list.Add(key);
            }
            else {
                keymap.Add(action, new List<KeyCode> {key});
            }
        }

        public static List<KeyCode> getKeyFromAction(Action a) {
            if (instance == null) {
                instance = new GameActions();
            }
            return instance.keymap[a];
        }

        public static bool checkAction(Action a, inputCheckType f) {
            iterable = getKeyFromAction(a);
            for (int i = 0; i < iterable.Count; ++i) {
                if(f(iterable[i])) {
                    return true;
                }
            }
            return false;
        }

        private GameActions() {
            registerAction(Action.LEFT, KeyCode.LeftArrow);
            registerAction(Action.LEFT, KeyCode.A);

            registerAction(Action.FORWARD, KeyCode.UpArrow);
            registerAction(Action.FORWARD, KeyCode.W);

            registerAction(Action.BACK, KeyCode.DownArrow);
            registerAction(Action.BACK, KeyCode.S);

            registerAction(Action.RIGHT, KeyCode.RightArrow);
            registerAction(Action.RIGHT, KeyCode.D);

            registerAction(Action.QUIT, KeyCode.Escape);

            registerAction(Action.USE, KeyCode.Mouse0);
            registerAction(Action.USE, KeyCode.Mouse1);

            registerAction(Action.DEBUG, KeyCode.P);
        }
    }
}
