using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CarStuff.BindingInfo;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace CarStuff
{
    [BepInPlugin("Pandemonius.FortniteMod", "FortniteMod", "1.2.0")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class Project5 : BaseUnityPlugin
    {
        static internal Project5 Instance;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        private readonly Harmony harmony = new Harmony("Pandemonius.FortniteMod");
        public static bool InPhysics = false;
        public static PlayerPhysicsRegion carphysics;
        public static bool ChangeGrav = false;
        public static bool SetupInput = false;
        public static bool isOutside = true;
        public static bool Jumping = false;
        public static float fall = 0f;
        public static EntranceTeleport[] AllTeleports;
        public static EntranceTeleport[] outsideTeleports;
        public static EntranceTeleport[] insideTeleports;
        internal static gravbinds inputtime;
        //public static WheelCollider bumper1;
        //public static WheelCollider bumper2;
        //public static WheelCollider bumper3;
        public static ConfigFile BepInExConfig()
        {
            return Instance.Config;
        }
        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            CarStuff.Config.Instance.Setup();
            harmony.PatchAll();
            inputtime = new gravbinds();
            if ((CarStuff.Config.Instance.ManualSelect.Value == true) & (CarStuff.Config.Instance.WasConfigFixed.Value == false))
            {
                CarStuff.Config.Instance.ManualSelect.Value = false;
                CarStuff.Config.Instance.WasConfigFixed.Value = false;
            }
        }
    }
}
