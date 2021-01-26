using Better_Camera_Control;
using System;
using UIExpansionKit.API;
using UnityEngine;
using VRC.UserCamera;
using MelonLoader;


[assembly: MelonInfo(typeof(BetterCameraControl), "Better Camera Control", "1.0", "Slayer6409")]
[assembly: MelonGame("VRChat", "VRChat")]


namespace Better_Camera_Control
{

    //Thanks to nitrog0d and loukylor for help with this
    //My code is probably a bit weird, but oh well it works.

    public class BetterCameraControl : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Log("Better Camera Control, by Slayer, Started.");
            
            ICustomShowableLayoutedMenu cm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu3Columns);
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle Camera Movement",toggleCameraMovement);
        }

        public override void OnUpdate()
        {
            player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            if (player == null) return;
            //-1 to 1
            

            if (CameraMovement)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button9))
                {
                    if (turnMode) turnMode = false;
                    else turnMode = true;
                }

                if (turnMode)
                {
                    float movementMod = 2.0f;
                    if (Input.GetAxis("Horizontal") > .2f) rotate(Input.GetAxis("Horizontal") * movementMod);
                    if (Input.GetAxis("Horizontal") < -.2f) rotate(Input.GetAxis("Horizontal") * movementMod);
                    if (Input.GetAxis("Vertical") < -.2f) rotateVert(Input.GetAxis("Vertical") * movementMod);
                    if (Input.GetAxis("Vertical") > .2f) rotateVert(Input.GetAxis("Vertical") * movementMod);
                }
                else
                {
                    float movementMod = 15;
                    if (Input.GetAxis("Horizontal") > .2f) moveHorizontal(Input.GetAxis("Horizontal") / movementMod);
                    if (Input.GetAxis("Horizontal") < -.2f) moveHorizontal(Input.GetAxis("Horizontal") / movementMod);
                    if (Input.GetAxis("Vertical") < -.2f) moveBack(Input.GetAxis("Vertical") / movementMod);
                    if (Input.GetAxis("Vertical") > .2f) moveForward(Input.GetAxis("Vertical") / movementMod);
                }
                //moveHorizontal(float speed)
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > .2f) rotateVert(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal")*1.5f);
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < -.2f) rotateVert(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal")*1.5f);
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < -.2f) moveUp(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") / 12);
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > .2f) moveDown(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") / 12);
            }
        }
        

        private void toggleCameraMovement()
        {
            if (CameraMovement)
            {
                
                CameraMovement = false;
                player.field_Private_VRCPlayerApi_0.SetWalkSpeed(4);
                player.field_Private_VRCPlayerApi_0.SetRunSpeed(8);
                player.field_Private_VRCPlayerApi_0.SetStrafeSpeed(4);

            }
            else
            {
                var cam = getCam();
                if (cam != null)
                {
                    while (cam.prop_EnumPublicSealedvaAtLoWoCO5vUnique_0 != EnumPublicSealedvaAtLoWoCO5vUnique.World)
                    {
                        cam.viewFinder.transform.Find("PhotoControls/Left_Space").GetComponent<CameraInteractable>().Interact();
                    }
                }
                CameraMovement = true;
                player.field_Private_VRCPlayerApi_0.SetWalkSpeed(0);
                player.field_Private_VRCPlayerApi_0.SetRunSpeed(0);
                player.field_Private_VRCPlayerApi_0.SetStrafeSpeed(0);
            }
        }

        private void moveBack(float speed)
        {
            var camController = getCam();
            var camRot = worldCameraQuaternion.ToEuler();
            if (camController == null) return;
            worldCameraVector += new Vector3((float)Math.Sin(camRot.y) * speed, 0f, (float)Math.Cos(camRot.y) * speed);

        }

        private void moveForward(float speed)
        {
            var camController = getCam();
            var camRot = worldCameraQuaternion.ToEuler();
            if (camController == null) return;
            worldCameraVector += new Vector3((float)Math.Sin(camRot.y) * speed, 0f, (float)Math.Cos(camRot.y) * speed);

        }
        private void moveHorizontal(float speed)
        {
            var camController = getCam();
            var camRot = worldCameraQuaternion.ToEuler();
            if (camController == null) return;
            worldCameraVector += Vector3.Cross(Vector3.up,new Vector3((float)Math.Sin(camRot.y), 0f, (float)Math.Cos(camRot.y))*speed);

        }

        private void moveUp(float speed)
        {
            var camController = getCam();
            if (camController == null) return;
            worldCameraVector += new Vector3(0f, speed, 0f);

        }
        private void moveDown(float speed)
        {
            var camController = getCam();
            if (camController == null) return;
            worldCameraVector += new Vector3(0f, speed, 0f);

        }


        public void rotate(float dir)
        {
            var camController = getCam();
            if (camController == null) return;
            worldCameraQuaternion *= Quaternion.Euler(0f, dir, 0f);
        }
        public void rotateVert(float dir)
        {
            var camController = getCam();
            if (camController == null) return;
            worldCameraQuaternion *= Quaternion.Euler(dir, 0f, 0f);
        }

        public static UserCameraController getCam()
        {
            return UserCameraController.field_Internal_Static_UserCameraController_0;
        }

        public static Vector3 worldCameraVector
        {
            get
            {
                return getCam().field_Private_Vector3_0;
            }
            set
            {
                getCam().field_Private_Vector3_0 = value;
            }
        }

        public static Quaternion worldCameraQuaternion
        {
            get
            {
                return getCam().field_Private_Quaternion_0;
            }
            set
            {
                getCam().field_Private_Quaternion_0 = value;
            }
        }

        public static bool CameraMovement { get; set; } = false;
        public static bool turnMode { get; set; } = false;
        public static VRCPlayer player { get; set; }
        public static float mSpeed { get; set; } = 0.25f;

    }
}