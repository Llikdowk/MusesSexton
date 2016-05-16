using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class DriveCartBehaviour : CharacterBehaviour {
        private static readonly GameObject cart = GameObject.Find("Cart");
        private static readonly Transform forwardCart = cart.transform.GetChild(0).transform;
        private static readonly Vector3 playerOffset = Vector3.zero; //new Vector3(-.6f, .3f, -4.8f);

        private const float stepRotation = 45f;
        private float t0speedUp = 0f;
        private float t0speedDown = 0f;
        private float currentSpeed = 0f;
        private const float initialSpeed = .5f;
        private const float acceleration = .25f;
        private const float deceleration = .5f;
        private const float maxSpeed = .25f;
        private bool timeSpeedUpRegistered = false;
        private bool timeSpeedDownRegistered = false;
        private readonly MouseLook mouseLook;
        private Vector3 direction;

        public DriveCartBehaviour(GameObject character) : base(character) {
            Debug.Log("DRIVING!");
            //cart.transform.parent = character.transform;
            mouseLook = new MouseLook();
            mouseLook.Init(character.transform, Camera.main.transform);
            mouseLook.XSensitivity = 1f;
            mouseLook.YSensitivity = 1f;
            mouseLook.smooth = true;

        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
        }

        public override void destroy() {
            cart.transform.parent = null;
        }

        public override void run() {
            checkStateChange();

            mouseLook.LookRotation(character.transform, Camera.main.transform);
            movementUpdate();
        }

        private void checkStateChange() {
            if(GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
            }
        }

        private void moveLeftRightCheck() {
            // WORK ON THIS!
            if (GameActions.checkAction(Action.LEFT, Input.GetKey)) {
                direction = -forwardCart.transform.right * stepRotation * Time.deltaTime;
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, -stepRotation * Time.deltaTime);
            }
            else if (GameActions.checkAction(Action.LEFT, Input.GetKeyUp)) {
                direction = Vector3.zero;
            }
            if (GameActions.checkAction(Action.RIGHT, Input.GetKey)) {
                direction = forwardCart.transform.right * stepRotation * Time.deltaTime;
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, stepRotation * Time.deltaTime);
            }
            else if (GameActions.checkAction(Action.RIGHT, Input.GetKeyUp)) {
                direction = Vector3.zero;
            }
        }

        private void movementUpdate() {

            cartUpdatePosition();

            if (GameActions.checkAction(Action.FORWARD, Input.GetKey)) {
                moveLeftRightCheck();
                timeSpeedDownRegistered = false;

                if (!timeSpeedUpRegistered) {
                    t0speedUp = Time.time;
                    timeSpeedUpRegistered = true;
                }
                moveForward();
            }
            else if (GameActions.checkAction(Action.FORWARD, Input.GetKeyUp)) {
                moveLeftRightCheck();
                timeSpeedUpRegistered = false;
                if (!timeSpeedDownRegistered) {
                    t0speedDown = Time.time;
                    timeSpeedDownRegistered = true;
                }
            }
            if (timeSpeedDownRegistered) {
                decelerateForward();
            }
        }

        private void cartUpdatePosition() {
            cart.transform.position = character.transform.localPosition + playerOffset;
        }

        private void moveForward() {
            float t = Time.time - t0speedUp;
            currentSpeed = Mathf.Min(maxSpeed, initialSpeed * acceleration * t * t);
            character.transform.position += Vector3.Normalize(forwardCart.transform.forward + direction) * currentSpeed;
        }

        private void decelerateForward() {
            float t = Time.time - t0speedDown;
            currentSpeed -= Mathf.Max(0, currentSpeed*deceleration * t * t);
            character.transform.position += Vector3.Normalize(forwardCart.transform.forward + direction) * currentSpeed;
        }
    }
}
