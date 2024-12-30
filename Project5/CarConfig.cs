using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Collections;
using Unity.Netcode;
using static Unity.Netcode.CustomMessagingManager;


namespace CarStuff
{
    public sealed class Config
    {
        #region Properties

        public new ConfigEntry<bool> Enabled { get; set; }

        public new ConfigEntry<bool> ManualSelect { get; set; }

        public new ConfigEntry<bool> WasConfigFixed { get; set; }

        public new ConfigEntry<bool> BuildingCar { get; set; }

        private static Config instance = null;
        public static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();

                return instance;
            }
        }
        #endregion

        public void Setup()
        {
            Enabled = Project5.BepInExConfig().Bind("General", "Enabled", true, "Enables the fortnite");
            ManualSelect = Project5.BepInExConfig().Bind("General", "Gravity select mode", false, "use j, currently broken");
            WasConfigFixed = Project5.BepInExConfig().Bind("Dev", "ConfigFixed", false, "Manual select was on by default and i cant do much about it now so config to disable the config");
            BuildingCar = Project5.BepInExConfig().Bind("General", "Car go in building", true, "isnt effected by gravity control being enabled, car go building near door, dont be suprised if you fall out of the map or get stuck in a wall thats your fault my mod is flawless shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up shut up ");
        }
    }
}