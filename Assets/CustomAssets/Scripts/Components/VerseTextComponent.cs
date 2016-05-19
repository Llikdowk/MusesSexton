using System;
using UnityEngine;
using Assets.CustomAssets.Scripts.Player;

//should be renamed to: VerseTextComponent
namespace Assets.CustomAssets.Scripts.Components {
    public class VerseTextComponent : MonoBehaviour {


        public string[] verse = new string[4]; // 0-> masculine, 1-> femenine, 2-> plural, 3-> first person
        public Gender initGenderDisplayed = Gender.MASCULINE;
        private Transform orb;

        public Vector3 originalPosition { get; private set; }
        public Quaternion originalRotation { get; private set; }
        public Vector3 originalScale { get; private set; }
        public int index { get; private set; }

        public string getVerse() {
            if (Player.Player.getInstance().genderChosen != Gender.UNDECIDED) {
                return verse[(int)Player.Player.getInstance().genderChosen];
            } else {
                return verse[(int)initGenderDisplayed];
            }
        }

        public void resetOrigins() {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
        }

        public void Start () {
            orb = transform.GetChild(0);
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalScale = transform.localScale;
            index = (int)Char.GetNumericValue(name[name.Length - 1]);
        }
	
        public void Update () {
            Vector3 v = new Vector3(Mathf.Sin(Time.time + originalPosition.x + originalPosition.z) /4f, Mathf.Cos(Time.time + originalPosition.z + originalPosition.x), 0);
            orb.transform.localPosition = v;
        }
        
    }
}
