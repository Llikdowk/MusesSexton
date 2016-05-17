using System.Collections;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Components {
    public class TextSetComponent : MonoBehaviour {

        public float movementSpeed = 0.1f;
        private readonly Vector3 nearScale = new Vector3(.05f, .05f, .05f);
        private bool actionDone = false;

        public void resetAllToOrigin() {
            for (int i = 0; i < transform.childCount; ++i) {
                VerseTextComponent verse = transform.GetChild(i).GetComponent<VerseTextComponent>();
                verse.resetOrigins();
            }
        }

        public void updateTextGenders() {
            for (int i = 0; i < transform.childCount; ++i) {
                VerseTextComponent verse = transform.GetChild(i).GetComponent<VerseTextComponent>();
                verse.updateTextGender();
            }
        }

        public void updatePlayerState(int n) {
            Gender g = transform.GetChild(n).GetComponent<VerseTextComponent>().initGenderDisplayed;
            Player.Player p = Player.Player.getInstance();
            if (p.genderChosen == Gender.UNDECIDED) {
                p.genderChosen = g;
            }
            p.addVerse(transform.GetChild(n).GetComponent<VerseTextComponent>().verse[3]);
            p.versesSelectedCount++;
        }

        public void doGoToPlayer(Transform destination) {
            if (!actionDone) {
                Player.Player.getInstance().behaviour.cinematicMode(true);
                StartCoroutine(runToDestination(destination));
                actionDone = true;
            }
        }

        public void doGoToOrigin(int verseAvoided, Transform graveHollow) {
            if (actionDone) {
                StartCoroutine(runOrigin(verseAvoided, graveHollow));
                actionDone = false;
            }
        }

        private IEnumerator runToDestination(Transform destination) {
            int n = 0;
            foreach (Transform verse in transform) {
                StartCoroutine(moveToDestination(verse, destination, n));
                ++n;
                yield return new WaitForSeconds(.1f);
            }
            Player.Player.getInstance().behaviour.cinematicMode(false);
        }

        private IEnumerator moveToDestination(Transform verse, Transform destination, int n) {
            float t = 0f;
            Vector3 originalScale = verse.localScale;
            Vector3 originalPosition = verse.position;
            Quaternion originalRotation = verse.rotation;
            while (t < 1f) {
                t += movementSpeed;
                verse.position = Vector3.Slerp(originalPosition, destination.GetChild(n).position, t);
                verse.rotation = Quaternion.Slerp(originalRotation, destination.rotation, t);
                verse.localScale = Vector3.Slerp(originalScale, nearScale, t);
                yield return new WaitForSeconds(.016f);
            }
        }

        private IEnumerator moveToGraveHollow(Transform verse, Transform destination) {
            float t = 0f;
            Vector3 originalScale = verse.localScale;
            Vector3 originalPosition = verse.position;
            Quaternion originalRotation = verse.rotation;
            while (t < 1f) {
                t += movementSpeed;
                verse.position = Vector3.Slerp(originalPosition, destination.position, t);
                verse.rotation = Quaternion.Slerp(originalRotation, destination.rotation, t);
                verse.localScale = Vector3.Slerp(originalScale, nearScale, t);
                yield return new WaitForSeconds(.016f);
            }
        }

        private IEnumerator runOrigin(int childAvoided, Transform graveHollow) {
            for (int i = 0; i < transform.childCount; ++i) {
                if (i != childAvoided) {
                    StartCoroutine(moveToOrigin(transform.GetChild(i)));
                    yield return new WaitForSeconds(.25f);
                } else {
                    StartCoroutine(moveToGraveHollow(transform.GetChild(i), graveHollow));
                    yield return new WaitForSeconds(.25f);
                }
            }
        }

        private IEnumerator moveToOrigin(Transform verse) {
            float t = 0f;
            VerseTextComponent aux = verse.gameObject.GetComponent<VerseTextComponent>();
            Vector3 currentScale = verse.localScale;
            Vector3 currentPosition = verse.position;
            Quaternion currentRotation = verse.rotation;
            while (t < 1f) {
                t += movementSpeed;
                verse.position = Vector3.Slerp(currentPosition, aux.originalPosition, t);
                verse.rotation = Quaternion.Slerp(currentRotation, aux.originalRotation, t);
                verse.localScale = Vector3.Slerp(currentScale, aux.originalScale, t);
                yield return new WaitForSeconds(.016f);
            }
        }
    }
}
