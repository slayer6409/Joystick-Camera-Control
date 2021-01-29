using Joystick_Camera_Control;
using System;
using UIExpansionKit.API;
using UnityEngine;
using VRC.UserCamera;
using MelonLoader;


[assembly: MelonInfo(typeof(JoystickCameraControl), "Joystick Camera Control", "1.1", "Slayer6409")]
[assembly: MelonGame("VRChat", "VRChat")]


namespace Joystick_Camera_Control
{

    //Thanks to nitrog0d, loukylor, and Janni9009 for help with this
    //My code is probably a bit weird, but oh well it works.

    public class JoystickCameraControl : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Log("Joystick Camera Control, by Slayer, Started.");
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle Camera Movement", toggleCameraMovement);
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.CameraQuickMenu).AddSimpleButton("Switch Turn Mode", switchTurnMode);
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.CameraQuickMenu).AddSimpleButton("Local Camera Mode", swapMode);
        }
        public override void OnUpdate()
        {
            
            if (CameraMovement)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button9)||Input.GetKeyDown(KeyCode.JoystickButton9))
                {
                    turnMode = !turnMode;
                }
                if (turnMode)
                {
                    var turnMovementMod = 1.5f;
                    if (Input.GetAxis("Horizontal") > .2f) rotate(Input.GetAxis("Horizontal") * turnMovementMod);
                    if (Input.GetAxis("Horizontal") < -.2f) rotate(Input.GetAxis("Horizontal") * turnMovementMod);
                    if (Input.GetAxis("Vertical") < -.2f) rotateVert(Input.GetAxis("Vertical") * turnMovementMod);
                    if (Input.GetAxis("Vertical") > .2f) rotateVert(Input.GetAxis("Vertical") * turnMovementMod);
                }
                else
                {
                    var movementMod = 1.75f;
                    if (Input.GetAxis("Horizontal") > .2f) moveHorizontal(Input.GetAxis("Horizontal") * movementMod);
                    if (Input.GetAxis("Horizontal") < -.2f) moveHorizontal(Input.GetAxis("Horizontal") * movementMod);
                    if (Input.GetAxis("Vertical") < -.2f) moveFrontBack(Input.GetAxis("Vertical") * movementMod);
                    if (Input.GetAxis("Vertical") > .2f) moveFrontBack(Input.GetAxis("Vertical") * movementMod);
                }
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > .2f)
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < -.2f) 
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < -.2f) moveY(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical"));
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > .2f) moveY(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical"));
            }
        }

        private void switchTurnMode()
        {
            turnMode = !turnMode;
        }
        private void toggleCameraMovement()
        {
            player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            if (player == null) return;
            if (CameraMovement)
            {
                CameraMovement = false;
                player.field_Private_VRCPlayerApi_0.SetWalkSpeed(walkSpeed);
                player.field_Private_VRCPlayerApi_0.SetRunSpeed(runSpeed);
                player.field_Private_VRCPlayerApi_0.SetStrafeSpeed(strafeSpeed);
            }
            else
            {
                var cam = UserCameraController.field_Internal_Static_UserCameraController_0;
                if (cam == null) return;
                while (cam.prop_EnumPublicSealedvaAtLoWoCO5vUnique_0 != EnumPublicSealedvaAtLoWoCO5vUnique.World)
                {
                    cam.viewFinder.transform.Find("PhotoControls/Left_Space").GetComponent<CameraInteractable>().Interact();
                }
                walkSpeed = player.field_Private_VRCPlayerApi_0.GetWalkSpeed();
                runSpeed = player.field_Private_VRCPlayerApi_0.GetRunSpeed();
                strafeSpeed = player.field_Private_VRCPlayerApi_0.GetStrafeSpeed();
                CameraMovement = true;
                player.field_Private_VRCPlayerApi_0.SetWalkSpeed(0);
                player.field_Private_VRCPlayerApi_0.SetRunSpeed(0);
                player.field_Private_VRCPlayerApi_0.SetStrafeSpeed(0);
            }
        }
        private void swapMode()
        {
            if (CameraMovement) toggleCameraMovement();
            var cam = UserCameraController.field_Internal_Static_UserCameraController_0;
            if (cam != null) cam.prop_EnumPublicSealedvaAtLoWoCO5vUnique_0 = EnumPublicSealedvaAtLoWoCO5vUnique.Local;
        }
        private void moveFrontBack(float speed)
        {
            var camController = UserCameraController.field_Internal_Static_UserCameraController_0;
            var camRot = worldCameraQuaternion.ToEuler();
            if (camController == null) return;
            worldCameraVector += new Vector3((float)Math.Sin(camRot.y) * (speed * Time.deltaTime), 0f, (float)Math.Cos(camRot.y) * (speed * Time.deltaTime));

        }
        private void moveHorizontal(float speed)
        {
            var camController = UserCameraController.field_Internal_Static_UserCameraController_0;
            var camRot = worldCameraQuaternion.ToEuler();
            if (camController == null) return;
            worldCameraVector += Vector3.Cross(Vector3.up,new Vector3((float)Math.Sin(camRot.y), 0f, (float)Math.Cos(camRot.y))*(speed*Time.deltaTime));

        }
        private void moveY(float speed)
        {
            var camController = UserCameraController.field_Internal_Static_UserCameraController_0;
            if (camController == null) return;
            worldCameraVector += new Vector3(0f, speed * Time.deltaTime, 0f);

        }
        public void rotate(float dir)
        {
            var camController = UserCameraController.field_Internal_Static_UserCameraController_0;
            if (camController == null) return;
            worldCameraQuaternion *= Quaternion.Euler(0f, dir, 0f);
        }
        public void rotateVert(float dir)
        {
            var camController = UserCameraController.field_Internal_Static_UserCameraController_0;
            if (camController == null) return;
            worldCameraQuaternion *= Quaternion.Euler(dir, 0f, 0f);
        }

        

        public static Vector3 worldCameraVector
        {
            get
            {
                return UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Vector3_0;
            }
            set
            {
                UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Vector3_0 = value;
            }
        }

        public static Quaternion worldCameraQuaternion
        {
            get
            {
                return UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Quaternion_0;
            }
            set
            {
                UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Quaternion_0 = value;
            }
        }

        public static bool CameraMovement { get; set; } = false;
        public static bool turnMode { get; set; } = false;
        public static VRCPlayer player { get; set; }
        public static float walkSpeed { get; set; }
        public static float runSpeed { get; set; }
        public static float strafeSpeed { get; set; }
        public static float mSpeed { get; set; } = 0.15f;

    }
}