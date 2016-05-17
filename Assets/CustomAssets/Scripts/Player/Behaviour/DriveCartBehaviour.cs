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

        private readonly CharacterController characterController;
        private const float stepRotation = 12.25f;
        private float t0speedUp = 0f;
        private float t0speedDown = 0f;
        private float currentSpeed = 0f;
        private const float initialSpeed = .5f;
        private const float acceleration = .25f;
        private const float deceleration = .5f;
        private const float maxSpeed = .15f;
        private const float gravity = 0.0981f;
        private bool timeSpeedUpRegistered = false;
        private bool timeSpeedDownRegistered = false;
        private readonly MouseLook mouseLook;
        private Vector3 direction;
        //from fpscontroller:
        private float radius = 2f;
        private float height = 2f;

        public DriveCartBehaviour(GameObject character) : base(character) {
            Debug.Log("DRIVING!");
            //cart.transform.parent = character.transform;
            mouseLook = new MouseLook();
            mouseLook.Init(character.transform, Camera.main.transform);
            mouseLook.XSensitivity = 1f;
            mouseLook.YSensitivity = 1f;
            mouseLook.smooth = true;
            characterController = character.GetComponent<CharacterController>();
            direction = cart.transform.forward;

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

        private void movementUpdate() {

            cartUpdatePosition();

            RaycastHit hitInfo;
            if (Physics.SphereCast(character.transform.position, radius, Vector3.down, out hitInfo,
                               height / 2f, ~0, QueryTriggerInteraction.Ignore)) {
                direction = Vector3.ProjectOnPlane(direction, hitInfo.normal).normalized;
                Vector3 localPlayerPos = character.transform.localPosition;
                //TODO: ROTATE VERTICAL ACCORDING TO THE NORMAL
            } else {
                //direction += Vector3.down * gravity; //fixme
            }

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

        private void moveLeftRightCheck() {
            // WORK ON THIS!
            if (GameActions.checkAction(Action.LEFT, Input.GetKey)) {
                direction += -forwardCart.transform.right * stepRotation;
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, -stepRotation * Time.deltaTime);
            }
            else if (GameActions.checkAction(Action.LEFT, Input.GetKeyUp)) {
                direction = forwardCart.transform.forward;
            }
            if (GameActions.checkAction(Action.RIGHT, Input.GetKey)) {
                direction += forwardCart.transform.right * stepRotation;
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, stepRotation * Time.deltaTime);
            }
            else if (GameActions.checkAction(Action.RIGHT, Input.GetKeyUp)) {
                direction = forwardCart.transform.forward;
            }
        }

        private void moveForward() {
            float t = Time.time - t0speedUp;
            currentSpeed = Mathf.Min(maxSpeed, initialSpeed * acceleration * t * t);
            character.transform.position += direction*Time.deltaTime * currentSpeed;
        }

        private void decelerateForward() {
            float t = Time.time - t0speedDown;
            currentSpeed -= Mathf.Max(0, currentSpeed*deceleration * t * t);
            character.transform.position += direction*Time.deltaTime * currentSpeed;
        }
    }
}
