﻿
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;
using Assets.CustomAssets.Scripts.Components;

namespace Assets.CustomAssets.Scripts.Player.Behaviour {
    public class DriveCartBehaviour : CharacterBehaviour {
        private static readonly GameObject cart = GameObject.Find("Cart");
        private static readonly Transform forwardCart = cart.transform.GetChild(0).transform;
        private static readonly Vector3 playerOffset = Vector3.zero; //new Vector3(-.6f, .3f, -4.8f);

        private readonly CharacterController characterController;
        private const float stepRotation = 90f;
        private float t0speedUp = 0f;
        private float t0speedDown = 0f;
        private float currentSpeed = 0f;
        //private const float initialSpeed = .35f;
        private const float acceleration = .01f;
        private const float deceleration = .2f;
        private const float maxSpeed = .125f;
        private bool timeSpeedUpRegistered = false;
        private bool timeSpeedDownRegistered = false;
        private readonly FirstPersonController fps;

        public DriveCartBehaviour(GameObject character) : base(character) {
            Debug.Log("DRIVING!");
            //cart.transform.parent = character.transform;
            //mouseLook = new MouseLook();
            characterController = character.GetComponent<CharacterController>();

            fps = character.AddComponent<FirstPersonController>();
            var mouseLook = fps.m_MouseLook;
            configureController();
            mouseLook.Init(character.transform, Camera.main.transform);
            mouseLook.XSensitivity = 1f;
            mouseLook.YSensitivity = 1f;
            mouseLook.smooth = true;
        }

        private void configureController() {
            fps.m_WalkSpeed = 0;
            fps.m_RunSpeed = 0;
            fps.m_JumpSpeed = 0;
            fps.m_GravityMultiplier = 1;
        }

        public override void cinematicMode(bool enabled) {
            base.cinematicMode(enabled);
        }

        public override void destroy() {
            UnityEngine.Object.Destroy(fps);
            cart.transform.parent = null;
        }

        public override void run() {
            checkStateChange();

            //mouseLook.LookRotation(character.transform, Camera.main.transform);
            movementUpdate();
        }

        private void checkStateChange() {
            if(GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                Player.getInstance().behaviour = new ExploreWalkBehaviour(character);
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

        private void moveLeftRightCheck() {
            
            if (GameActions.checkAction(Action.LEFT, Input.GetKey)) {
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, -stepRotation * currentSpeed * 2f * Time.deltaTime);
            }
            if (GameActions.checkAction(Action.RIGHT, Input.GetKey)) {
                Vector3 localPlayerPos = character.transform.localPosition;
                cart.transform.RotateAround(localPlayerPos, cart.transform.up, stepRotation * currentSpeed * 2f * Time.deltaTime);
            }
        }

        private void moveForward() {
            float t = Time.time - t0speedUp;
            currentSpeed = Mathf.Min(maxSpeed, currentSpeed + acceleration * t * t);
            character.transform.position += forwardCart.transform.forward * currentSpeed;
        }

        private void decelerateForward() {
            float t = Time.time - t0speedDown;
            currentSpeed = Mathf.Max(0, currentSpeed - deceleration * t * t);
            character.transform.position += forwardCart.transform.forward * currentSpeed;
        }
    }
}
