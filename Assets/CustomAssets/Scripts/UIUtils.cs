using UnityEngine;
using UnityEngine.UI;

namespace Assets.CustomAssets.Scripts {
    public static class UIUtils {
        //private static readonly Text infoInteractive = GameObject.Find("InfoText").GetComponent<Text>();
        private static readonly Image infoInteractive = GameObject.Find("InfoIcon").GetComponent<Image>();

        private static readonly Sprite burySprite = Resources.Load<Sprite>("Sprites/bury");
        private static readonly Sprite catchSprite = Resources.Load<Sprite>("Sprites/catch");
        private static readonly Sprite digSprite = Resources.Load<Sprite>("Sprites/dig");
        private static readonly Sprite driveSprite = Resources.Load<Sprite>("Sprites/drive");
        private static readonly Sprite landmarkSprite = Resources.Load<Sprite>("Sprites/landmark");
        private static readonly Sprite emptySprite = Resources.Load<Sprite>("Sprites/empty");

        private static bool setForClear = false;
        private static bool hasPainted = false;

        public static void forceClear() {
            infoInteractive.sprite = emptySprite;
        }

        public static void markToClear() {
            /*
            if (mask == 0) {
                infoInteractive.sprite = emptySprite;
            }
            */
            setForClear = true;
        }

        public static void checkForClearAction() {
            if (setForClear && !hasPainted) {
                infoInteractive.sprite = emptySprite;
            }
            setForClear = false;
            hasPainted = false;
        }

        public static void drive() {
            infoInteractive.sprite = driveSprite;
            hasPainted = true;
        }

        public static void landmarkFound() {
            infoInteractive.sprite = landmarkSprite;
            hasPainted = true;
        }

        public static void catchCoffin() {
            infoInteractive.sprite = catchSprite;
            hasPainted = true;
        }

        public static void dig() {
            infoInteractive.sprite = digSprite;
            hasPainted = true;
        }

        public static void buryCoffin() {
            infoInteractive.sprite = burySprite;
            hasPainted = true;
        }

    }
}
