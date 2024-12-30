using GameNetcodeStuff;
using HarmonyLib;
using System;
using Unity;
using UnityEngine;
using CarStuff;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Discord;

namespace CarStuff.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    public class CarPatchBuilding
    {
        public static float timeAtLastUsingEntrance = 0f;
        public static Transform ClosestPoint = new Transform();
        public static float MinMagnitude = 8f;
        public static float ClosestMagnitude = MinMagnitude;
        public static bool updated = false;
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPostfix]
        public static void TeleportCar(VehicleController __instance)
        {
            if (Config.Instance.BuildingCar.Value == false) return;
            if (!__instance.localPlayerInControl) return;
            Project5.isOutside = __instance.mainRigidbody.transform.position.y > -80f;
            Project5.Logger.LogInfo($"Outside = {Project5.isOutside}");
            //if (!CarInBuilding.isOutside & !updated)
            //{
            //    updated = true;
            //    ((NetworkBehaviour)__instance).NetworkObject.SynchronizeTransform = true;
            //    ((NetworkBehaviour)__instance).NetworkObject.Despawn(false);
            //    ((NetworkBehaviour)__instance).NetworkObject.transform.parent.localScale = new Vector3(0.59f, 0.59f, 0.59f);
            //    ((NetworkBehaviour)__instance).NetworkObject.Spawn(false);
            //}
            //else if (CarInBuilding.isOutside & updated)
            //{
            //    updated = false;
            //    ((NetworkBehaviour)__instance).NetworkObject.SynchronizeTransform = true;
            //    ((NetworkBehaviour)__instance).NetworkObject.Despawn(false);
            //    ((NetworkBehaviour)__instance).NetworkObject.transform.parent.localScale = new Vector3(1.18f, 1.18f, 1.18f);
            //    ((NetworkBehaviour)__instance).NetworkObject.Spawn(false);
            //}
            if (__instance.IsOwner & !__instance.carDestroyed & !StartOfRound.Instance.inShipPhase)
            {
                if ((Time.realtimeSinceStartup - timeAtLastUsingEntrance) > 1.8f)
                {
                    ClosestMagnitude = MinMagnitude;
                    if (!Project5.isOutside)
                    {
                        int i = 0;
                        EntranceTeleport[] array3 = Project5.insideTeleports;
                        foreach (EntranceTeleport val3 in array3)
                        {
                            if (Vector3.Distance(__instance.mainRigidbody.position, val3.entrancePoint.transform.position) < ClosestMagnitude)
                            {
                                ClosestMagnitude = Vector3.Distance(__instance.mainRigidbody.position, val3.entrancePoint.transform.position);
                                ClosestPoint = Project5.outsideTeleports[i].transform;
                                //if (val3.isEntranceToBuilding && !CarInBuilding.isOutside && val3.entranceId != 0) ClosestPoint.Rotate(0f, 180f, 0f, (Space)1);
                            }
                            i += 1;
                        }
                        //CarInBuilding.Logger.LogInfo($"closest = min is {ClosestMagnitude == MinMagnitude}");
                        if (ClosestMagnitude == MinMagnitude) return;

                        //CarInBuilding.Logger.LogInfo($"teleported");
                        timeAtLastUsingEntrance = Time.realtimeSinceStartup;
                        __instance.mainRigidbody.isKinematic = true;
                        __instance.transform.position = ClosestPoint.position + (ClosestPoint.transform.right * 12);
                        __instance.transform.rotation = Quaternion.Euler(0, ClosestPoint.eulerAngles.y + 90, 0);
                        __instance.syncedPosition = ClosestPoint.position + (ClosestPoint.transform.right * 12);
                        __instance.syncedRotation = Quaternion.Euler(0, ClosestPoint.eulerAngles.y + 90, 0);
                    }
                    else
                    {
                        int i = 0;
                        EntranceTeleport[] array2 = Project5.outsideTeleports;
                        foreach (EntranceTeleport val2 in array2)
                        {
                            if (Vector3.Distance(__instance.mainRigidbody.position, val2.entrancePoint.transform.position) < ClosestMagnitude)
                            {
                                ClosestMagnitude = Vector3.Distance(__instance.mainRigidbody.position, val2.entrancePoint.transform.position);
                                ClosestPoint = Project5.insideTeleports[i].transform;
                                //if (val2.isEntranceToBuilding && !CarInBuilding.isOutside && val2.entranceId != 0) ClosestPoint.Rotate(0f, 180f, 0f, (Space)1);
                            }
                            i += 1;
                        }
                        //CarInBuilding.Logger.LogInfo($"closest = min is {ClosestMagnitude == MinMagnitude}");
                        if (ClosestMagnitude == MinMagnitude) return;

                        //CarInBuilding.Logger.LogInfo($"teleported");
                        timeAtLastUsingEntrance = Time.realtimeSinceStartup;
                        __instance.mainRigidbody.isKinematic = true;
                        __instance.transform.position = ClosestPoint.position + (ClosestPoint.transform.right * 12);
                        __instance.transform.rotation = Quaternion.Euler(0, ClosestPoint.eulerAngles.y + 90, 0);
                        __instance.syncedPosition = ClosestPoint.position + (ClosestPoint.transform.right * 12);
                        __instance.syncedRotation = Quaternion.Euler(0, ClosestPoint.eulerAngles.y + 90, 0);
                    }
                }
            }
            else
            {
                timeAtLastUsingEntrance = Time.realtimeSinceStartup;
            }
        }

    }

    [HarmonyPatch(typeof(RoundManager))]
    public class EntrancePatchCar
    {
        [HarmonyPatch("SetLevelObjectVariables")]
        [HarmonyPostfix]
        private static void GetTeleports()
        {
            Project5.AllTeleports = UnityEngine.Object.FindObjectsOfType<EntranceTeleport>();
            Project5.outsideTeleports = (EntranceTeleport[])(object)new EntranceTeleport[Project5.AllTeleports.Length / 2];
            Project5.insideTeleports = (EntranceTeleport[])(object)new EntranceTeleport[Project5.AllTeleports.Length / 2];
            for (int i = 0; i < Project5.AllTeleports.Length; i++)
            {
                int entranceId = Project5.AllTeleports[i].entranceId;
                if (Project5.AllTeleports[i].isEntranceToBuilding)
                {
                    Project5.outsideTeleports[entranceId] = Project5.AllTeleports[i];
                }
                else
                {
                    Project5.insideTeleports[entranceId] = Project5.AllTeleports[i];
                }
            }
        }
    }

[HarmonyPatch(typeof(VehicleController))]
    public class CarControl
    {
        public static Vector3 grav = Physics.gravity;
        public static RaycastHit Carhit;
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void CarGravity(VehicleController __instance)
        {
            if (Config.Instance.Enabled.Value == true)
            {
                Project5.carphysics = __instance.physicsRegion;
                __instance.physicsRegion.maxTippingAngle = 2147483647;
                if (Config.Instance.ManualSelect.Value == true)
                {
                    if (Project5.ChangeGrav & (Physics.Raycast(__instance.transform.position, GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.forward * 1f, out Carhit, 2048f, StartOfRound.Instance.collidersRoomDefaultAndFoliage, QueryTriggerInteraction.Ignore))) 
                    {
                        Physics.gravity = Quaternion.Euler(Carhit.normal) * grav;
                        Project5.ChangeGrav = false;
                    }
                    else Project5.ChangeGrav = false;                                                                                                                                                                                                                                                                          
                }
                else
                {
                    Project5.ChangeGrav = false;
                    if (Physics.Raycast(__instance.transform.position, __instance.transform.up * -1f, out Carhit, 6f, StartOfRound.Instance.collidersAndRoomMask, QueryTriggerInteraction.Ignore)) Physics.gravity = __instance.transform.rotation * grav;
                }
            }
            else { Physics.gravity = grav; __instance.physicsRegion.maxTippingAngle = 180; }
        }
        //[HarmonyPatch("Update")]
        //[HarmonyPostfix]
        //private static void CarBumperGrab(VehicleController __instance)
        //{
        //    if (Project5.bumper1 != null)
        //    {
        //        Project5.bumper1.motorTorque = __instance.otherWheels[1].motorTorque*3;
        //        Project5.bumper1.brakeTorque = __instance.otherWheels[1].brakeTorque * 3;
        //    }
        //    if (Project5.bumper2 != null)
        //    {
        //        Project5.bumper1.motorTorque = __instance.otherWheels[1].motorTorque * 3;
        //        Project5.bumper1.brakeTorque = __instance.otherWheels[1].brakeTorque * 3;
        //    }
        //    if (Project5.bumper3 != null)
        //    {
        //        Project5.bumper1.motorTorque = __instance.otherWheels[1].motorTorque * 3;
        //        Project5.bumper1.brakeTorque = __instance.otherWheels[1].brakeTorque * 3;
        //    }
        //}
    }

    //[HarmonyPatch(typeof(VehicleController))]
    //public class CarWheelPatch
    //{
    //    [HarmonyPatch("Start")]
    //    [HarmonyPrefix]
    //    private static void CarBumper(VehicleController __instance)
    //    {
    //GameObject BumperWheel1 = Array.Find(__instance.otherWheels, wheel => wheel.name == "FrontWheel").gameObject;
    //GameObject BumperWheel2 = Array.Find(__instance.otherWheels, wheel => wheel.name == "FrontWheel (2)").gameObject;
    //GameObject BumperWheel3 = Array.Find(__instance.otherWheels, wheel => wheel.name == "FrontWheel (1)").gameObject;
    //var wheel1 = UnityEngine.Object.Instantiate(BumperWheel1, BumperWheel1.transform.parent.position + new Vector3(BumperWheel1.transform.localPosition.x, -0.552f, 4.13f), Quaternion.Euler(BumperWheel1.transform.parent.eulerAngles.x - 70, BumperWheel1.transform.parent.eulerAngles.y, BumperWheel1.transform.parent.eulerAngles.z), BumperWheel1.transform.parent);
    //var wheel2 = UnityEngine.Object.Instantiate(BumperWheel2, BumperWheel1.transform.parent.position + new Vector3(BumperWheel2.transform.localPosition.x, -0.552f, 4.13f), Quaternion.Euler(BumperWheel1.transform.parent.eulerAngles.x - 70, BumperWheel1.transform.parent.eulerAngles.y, BumperWheel1.transform.parent.eulerAngles.z), BumperWheel2.transform.parent);
    //var wheel3 = UnityEngine.Object.Instantiate(BumperWheel3, BumperWheel1.transform.parent.position + new Vector3(BumperWheel3.transform.localPosition.x, -0.552f, 4.13f), Quaternion.Euler(BumperWheel1.transform.parent.eulerAngles.x - 70, BumperWheel1.transform.parent.eulerAngles.y, BumperWheel1.transform.parent.eulerAngles.z), BumperWheel3.transform.parent);
    //wheel1.gameObject.GetComponent<WheelCollider>().radius = 0.8f;
    //wheel2.gameObject.GetComponent<WheelCollider>().radius = 0.8f;
    //wheel3.gameObject.GetComponent<WheelCollider>().radius = 0.8f;
    // wheel1.gameObject.GetComponent<WheelCollider>().wheelDampingRate = 0.3f;
    //wheel2.gameObject.GetComponent<WheelCollider>().wheelDampingRate = 0.3f;
    //wheel3.gameObject.GetComponent<WheelCollider>().wheelDampingRate = 0.3f;
    //wheel1.gameObject.GetComponent<WheelCollider>().suspensionDistance = 0.7f;
    //wheel2.gameObject.GetComponent<WheelCollider>().suspensionDistance = 0.7f;
    //wheel3.gameObject.GetComponent<WheelCollider>().suspensionDistance = 0.7f;
    //wheel1.gameObject.GetComponent<WheelCollider>().forceAppPointDistance = 1.6f;
    //wheel2.gameObject.GetComponent<WheelCollider>().forceAppPointDistance = 1.6f;
    //wheel3.gameObject.GetComponent<WheelCollider>().forceAppPointDistance = 1.6f;
    //Project5.bumper1 = wheel1.GetComponent<WheelCollider>();
    //Project5.bumper2 = wheel2.GetComponent<WheelCollider>();
    //Project5.bumper3 = wheel3.GetComponent<WheelCollider>();
    //WheelFrictionCurve wheelFrictionCurve = default(WheelFrictionCurve);
    //wheelFrictionCurve.extremumSlip = 0.99f;
    //wheelFrictionCurve.extremumValue = 1f;
    //wheelFrictionCurve.asymptoteSlip = 1f;
    //wheelFrictionCurve.asymptoteValue = 1f;
    //wheelFrictionCurve.stiffness = 2f;
    //wheel1.gameObject.GetComponent<WheelCollider>().forwardFriction = wheelFrictionCurve;
    //wheel2.gameObject.GetComponent<WheelCollider>().forwardFriction = wheelFrictionCurve;
    //wheel3.gameObject.GetComponent<WheelCollider>().forwardFriction = wheelFrictionCurve;
    //int resized = (__instance.otherWheels.Length + 3);
    //Array.Resize(ref __instance.otherWheels,resized);
    //__instance.otherWheels.SetValue(wheel1.gameObject.GetComponent<WheelCollider>(), resized - 3);
    //__instance.otherWheels.SetValue(wheel2.gameObject.GetComponent<WheelCollider>(), resized - 2);
    //__instance.otherWheels.SetValue(wheel3.gameObject.GetComponent<WheelCollider>(), resized - 1);

    //for (int i = 0; i <= __instance.otherWheels.Length; i++)
    //{
    //    Project5.Logger.LogInfo(__instance.otherWheels[i].gameObject.name);
    //}
    // }
    //}

    [HarmonyPatch(typeof(PlayerControllerB))]
    public class Jumper
    {
        [HarmonyPatch("IsPlayerNearGround")]
        [HarmonyPrefix]
        private static bool nearGround(PlayerControllerB __instance, ref bool __result)
        {
            __instance.interactRay = new Ray(__instance.transform.position, -__instance.transform.up);
            __result = Physics.Raycast(__instance.interactRay, 0.15f, StartOfRound.Instance.allPlayersCollideWithMask, QueryTriggerInteraction.Ignore);
            return false;
        }

        public static float TimeSinceJumping = 0f;
        [HarmonyPatch("Jump_performed")]
        [HarmonyPostfix]
        public static void JumpFix(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner) return;
            if (__instance.jumpCoroutine != null)
            {
                __instance.StopCoroutine(__instance.jumpCoroutine);
                TimeSinceJumping = Time.realtimeSinceStartup;
                __instance.playerBodyAnimator.SetBool("Jumping", value: true);
                Project5.Jumping = true;
            }
        }
        [HarmonyPatch("Crouch_performed")]
        [HarmonyPrefix]
        public static bool Croucher(ref InputAction.CallbackContext context, PlayerControllerB __instance)
        {
            if (!__instance.IsOwner) return true;
            if (context.performed && !__instance.quickMenuManager.isMenuOpen && !__instance.inSpecialInteractAnimation && __instance.IsPlayerNearGround() && !__instance.isTypingChat && !__instance.isJumping && !__instance.isSprinting)
            {
                __instance.crouchMeter = Mathf.Min(__instance.crouchMeter + 0.3f, 1.3f);
                __instance.Crouch(!__instance.isCrouching);
            }
            return false;
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void JumpHandler(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || !Project5.InPhysics) return;
            if (!__instance.isJumping)
            {
                if (__instance.IsPlayerNearGround()) CarStuff.Project5.fall = -7f; else CarStuff.Project5.fall = -32f;
            }
            if (Project5.Jumping)
            {
                if (((Time.realtimeSinceStartup - TimeSinceJumping) >= 0.15f) & ((Time.realtimeSinceStartup - TimeSinceJumping) < 0.25f))
                {
                    CarStuff.Project5.fall = 5f;
                }
                else if ((Time.realtimeSinceStartup - TimeSinceJumping) >= 0.25f)
                {
                    __instance.isJumping = false;
                    __instance.isFallingFromJump = true;
                    if (__instance.IsPlayerNearGround())
                    {
                        __instance.playerBodyAnimator.SetBool("Jumping", value: false);
                        __instance.isFallingFromJump = false;
                        __instance.PlayerHitGroundEffects();
                        Project5.Jumping = false;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysicsRegion))]
    public class PlayerFix
    {
        public static Vector3 playervel = new Vector3();
        public static float StepOffset = 0f;
        public static float Slope = 0f;
        public static float SlopeIntensity = 0f;
        public static PlayerControllerB LocalPlayer;
        public static LayerMask Layerinfo = 1073741824;
        public static Vector3 FakeWalkForce = Vector3.zero;
        public static Vector3 FakeExternalForce = Vector3.zero;
        public static float FakeSlopeModifier = 0f;
        public static float FakeSprintMultiplier = 0f;
        public static Vector3 vector;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void PlayerPatch(PlayerPhysicsRegion __instance)
        {
            if (__instance.maxTippingAngle != 2147483647) return;
            LocalPlayer = GameNetworkManager.Instance.localPlayerController;
            if (GameNetworkManager.Instance.localPlayerController.thisController.stepOffset > 0f) StepOffset = GameNetworkManager.Instance.localPlayerController.thisController.stepOffset;
            if (GameNetworkManager.Instance.localPlayerController.slopeIntensity > 0f) SlopeIntensity = GameNetworkManager.Instance.localPlayerController.slopeIntensity;
            if (GameNetworkManager.Instance.localPlayerController.thisController.slopeLimit < 180f) Slope = GameNetworkManager.Instance.localPlayerController.thisController.slopeLimit;
            if ((__instance == Project5.carphysics) & __instance.hasLocalPlayer & !GameNetworkManager.Instance.localPlayerController.inVehicleAnimation & (Config.Instance.Enabled.Value == true))
            {
                LocalPlayer.thisController.slopeLimit = 180;
                LocalPlayer.playerGroundNormal = Vector3.up;
                GameNetworkManager.Instance.localPlayerController.thisController.stepOffset = 0f; // should fix the getting glued to corners bullshit
                Project5.InPhysics = true;
                if (Physics.Raycast(LocalPlayer.thisController.transform.position + (__instance.transform.up * (StepOffset * 0.5f)), -__instance.transform.up, out LocalPlayer.hit, StepOffset, Layerinfo, QueryTriggerInteraction.Ignore))
                {
                    //playervel = Physics.gravity;
                    playervel = (GameNetworkManager.Instance.localPlayerController.physicsParent.transform.up * CarStuff.Project5.fall);
                    //if (GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.GetBool("FallNoJump"))
                    //{
                    //    GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetTrigger("ShortFallLanding");
                    //    GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("FallNoJump", value: false);
                    //}
                    //if (((GameNetworkManager.Instance.localPlayerController.moveInputVector.y > 0.1f & GameNetworkManager.Instance.localPlayerController.moveInputVector.y < -0.1f) || (GameNetworkManager.Instance.localPlayerController.moveInputVector.x > 0.1f & GameNetworkManager.Instance.localPlayerController.moveInputVector.x < -0.1f)) & !GameNetworkManager.Instance.localPlayerController.inSpecialInteractAnimation)
                    //{
                    //    GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("Walking", value: true);
                    //    if (GameNetworkManager.Instance.localPlayerController.moveInputVector.y < 0.2f & GameNetworkManager.Instance.localPlayerController.moveInputVector.y > -0.2f)
                    //    {
                    //        GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("Sideways", value: true);
                    //    }
                    //}
                    //else
                    //{
                    //    GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("Walking", value: false);
                    //    GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("Sideways", value: false);
                    //}
                }
                else
                {
                    //GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("crouching", value: false);
                    //GameNetworkManager.Instance.localPlayerController.playerBodyAnimator.SetBool("FallNoJump", value: true);
                    playervel = (GameNetworkManager.Instance.localPlayerController.physicsParent.transform.up * CarStuff.Project5.fall);
                    //playervel = Physics.gravity;f
                }
                GameNetworkManager.Instance.localPlayerController.thisController.Move(playervel);

                GameNetworkManager.Instance.localPlayerController.ResetFallGravity();

                //if (!Project5.Jumping && Physics.SphereCast(LocalPlayer.thisController.transform.position + (__instance.transform.up * ((StepOffset * 0.5f) + LocalPlayer.thisController.radius)), LocalPlayer.thisController.radius, -__instance.transform.up, out LocalPlayer.hit, StepOffset, Layerinfo, QueryTriggerInteraction.Ignore))
                //{
                    //LocalPlayer.thisController.SimpleMove(-__instance.transform.up * (LocalPlayer.hit.distance - (StepOffset * 0.5f))); // custom snapping because it only snaps directly down relative to the world
            }
            else
            {
                GameNetworkManager.Instance.localPlayerController.slopeIntensity = SlopeIntensity;
                GameNetworkManager.Instance.localPlayerController.thisController.slopeLimit = Slope;
                GameNetworkManager.Instance.localPlayerController.thisController.stepOffset = StepOffset;
                Project5.InPhysics = false;
            }
        }
    }
}
