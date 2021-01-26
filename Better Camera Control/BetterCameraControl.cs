using Better_Camera_Control;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Harmony;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UIExpansionKit.API;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using VRC.SDKBase;
using VRC.UserCamera;
using MelonLoader;
using Il2CppSystem.Collections.Generic;
using UnityEngine.XR;


// ...
[assembly: MelonInfo(typeof(BetterCameraControl), "Better Camera Control", "0.1", "Slayer")]
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
            //cm.AddSimpleButton("Move Back", moveBack);
            //cm.AddSimpleButton("Move Forward", moveForward);
            //cm.AddSimpleButton("Rotate Right", rotateRight);
            //cm.AddSimpleButton("Rotate Left", rotateLeft);
            //cm.AddSimpleButton("Move Up", moveUp);
            //cm.AddSimpleButton("Move Down", moveDown);
            //cm.AddSpacer();
            //cm.AddSimpleButton("Toggle Camera Movement", toggleCameraMovement);
            //cm.AddSimpleButton("Back", () => cm.Hide());
        }

        public override void OnUpdate()
        {
            player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            if (player == null) return;
            //-1 to 1

            if (CameraMovement)
            {

                if (Input.GetAxis("Horizontal") > 0f) rotate(Input.GetAxis("Horizontal"));
                if (Input.GetAxis("Horizontal") < 0f) rotate(Input.GetAxis("Horizontal"));
                if (Input.GetAxis("Vertical") < 0f) moveBack(Input.GetAxis("Vertical")/4);
                if (Input.GetAxis("Vertical") > 0f) moveForward(Input.GetAxis("Vertical")/4);
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > 0f)
                //if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < 0f)
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f) moveUp(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")/4);
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f) moveDown(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")/4);
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
        public static VRCPlayer player { get; set; }
        public static float mSpeed { get; set; } = 0.25f;

    }
}