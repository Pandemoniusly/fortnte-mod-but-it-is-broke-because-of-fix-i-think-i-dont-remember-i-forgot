using CarStuff.BindingInfo;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CarStuff.BindingInfo
{
//    public class AntiSlip : MonoBehaviour
//    {
        // this script pushes all rigidbodies that the character touches
 //       void OnControllerColliderHit(ControllerColliderHit hit)
 //       {
 //           if (Project5.InPhysics)
 //           {
 //               var __instance = hit.controller;
 //               Vector3 val = Vector3.zero;
 //               int num = 0;
 //               ControllerColliderHit val2 = default(ControllerColliderHit);
 //               val = hit.normal;
  //              val = (val).normalized;
  //                  Vector3 val3 = Vector3.ProjectOnPlane(((Component)__instance).transform.up, val);
  //                  Vector3 normalized = ((Vector3)(val3)).normalized;
  //                  Vector3 val4 = -val;
  //                  val3 = Physics.gravity;
  //                  Vector3 val5 = val4 * ((Vector3)(val3)).magnitude - Physics.gravity;
  //                      val5 = Vector3.ProjectOnPlane(val5, normalized);
  //                  __instance.attachedRigidbody.AddForce(val5, (ForceMode)5);
  //          }
  //      }
  //  }

    [HarmonyPatch(typeof(PlayerControllerB))]
    public class trueorrubbish
    {
        // Name this whatever you like. It needs to be called exactly once, so 
        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPrefix]
        public static void SetupKeybindCallbacks(PlayerControllerB __instance)
        {
            if (!Project5.SetupInput)
            {
                //__instance.gameObject.AddComponent<AntiSlip>();
                gravbinds.Instance.switchwall.performed += keybindHandler.wallswitch;
                Project5.SetupInput = true;
            }
        }
    }
    [HarmonyPatch(typeof(StartOfRound))]
    public class rubbishortrue
    {
        // Name this whatever you like. It needs to be called exactly once, so 
        [HarmonyPatch("OnLocalDisconnect")]
        [HarmonyPrefix]
        public static void Disconnectkeybinds()
        {
            if (Project5.SetupInput)
            {
                gravbinds.Instance.switchwall.performed -= keybindHandler.wallswitch;
                Project5.SetupInput = false;
            }
        }
    }
    public class keybindHandler
    {
        public static void wallswitch(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Project5.ChangeGrav = true;
            // Your executing code here
        }
    }
}