using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CustomAssets.Scripts.Components {
    public class CoffinDragged : MonoBehaviour {
        private Rigidbody rb;
        public void Awake() {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        public void OnEnable() {
            rb.isKinematic = false;
        }

        public void OnDisable() {
            freeze();
        }
        private void freeze() {
            rb.isKinematic = true;
        }
    }
}
