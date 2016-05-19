using System.Collections;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Components {
    public class TextSetComponent : MonoBehaviour {

        public float movementSpeed = 0.032f;
        public VerseTextComponent[] allOrbs = new VerseTextComponent[6];
        public Transform[] playerTextSlots = new Transform[6];

        public void Start() {
            for (int i = 0; i < allOrbs.Length; ++i) {
                VerseTextComponent orb = transform.GetChild(i).GetComponent<VerseTextComponent>();
                allOrbs[i] = orb;
            }

            Transform eyeSight = Player.Player.getInstance().eyeSight;
            for (int i = 0; i < eyeSight.childCount; ++i) {
                playerTextSlots[i] = eyeSight.GetChild(i);
            }
        }

        public void resetAllToOrigin() {
            for (int i = 0; i < allOrbs.Length; ++i) {
                allOrbs[i].resetOrigins();
            }
        }

        public void updatePlayerState(int n) {
            Gender g = transform.GetChild(n).GetComponent<VerseTextComponent>().initGenderDisplayed;
            Player.Player p = Player.Player.getInstance();
            if (p.genderChosen == Gender.UNDECIDED) {
                p.genderChosen = g;
            }
            p.addVerse(transform.GetChild(n).GetComponent<VerseTextComponent>().verse[(int)Gender.FIRST_PERSON]);
            p.versesSelectedCount++;
        }

        public string getTextOf(int n) {
            return transform.GetChild(n).GetComponent<VerseTextComponent>().getVerse();
        }

        public void moveAllOrbsTo(Transform destination, params endAnimationCallback[] f) {
            float wait = 0.0f;
            const float waitStep = 0.15f;
            foreach (VerseTextComponent orb in allOrbs) {
                moveSubjectTo(orb.transform, destination, wait, f);
                wait += waitStep;
            }
        }

        public void moveAllOrbsToOrigin(params endAnimationCallback[] f) {
            float wait = 0.0f;
            const float waitStep = 0.15f;
            foreach (VerseTextComponent orb in allOrbs) {
                Transform t = new GameObject().transform;
                t.position = orb.originalPosition;
                t.rotation = orb.originalRotation;
                t.localScale = orb.originalScale;
                endAnimationCallback enclosed_f = 
                    () => {
                        foreach (var x in f) x();
                        Destroy(t.gameObject);
                    };
                moveSubjectTo(orb.transform, t, wait, enclosed_f);
                wait += waitStep;
            }
        }

        public void moveSubjectTo(Transform subject, Transform destination, float waitForStart_s, params endAnimationCallback[] f) {
            Player.Player.getInstance().cinematic = true;
            StartCoroutine(doMoveTo(subject, destination, waitForStart_s, f));
        }

        private IEnumerator doMoveTo(Transform subject, Transform destination, float waitForStart_s, params endAnimationCallback[] f) {
            Transform origin = subject.transform;
            float t = 0.0f;
            yield return new WaitForSeconds(waitForStart_s);
            while (t < 1.0f) {
                t += movementSpeed;
                subject.position = Vector3.Slerp(origin.position, new Vector3(destination.position.x, subject.position.y, destination.position.z), t);
                subject.rotation = Quaternion.Slerp(origin.rotation, destination.rotation, t);
                yield return new WaitForSeconds(0.016f);
            }

            foreach (var x in f) x();
            Player.Player.getInstance().cinematic = false;
        }

    }
}
