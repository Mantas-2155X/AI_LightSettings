using HarmonyLib;

using BepInEx;
using BepInEx.Harmony;
using BepInEx.Configuration;

using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace AI_LightSettings {
    [BepInPlugin(nameof(AI_LightSettings), nameof(AI_LightSettings), VERSION)][BepInProcess("AI-Syoujyo")]
    public class AI_LightSettings : BaseUnityPlugin
    {
        public const string VERSION = "2.1.0";

        private const string ENABLE = "Enable Settings";
        private const string INTENSITY = "Intensity";
        private const string COLOR = "Color";
        
        private static List<Light> faceLights;
        private static Light backLight;
        private static Light camLight;
        
        private static Light dirBackLight;
        private static Light dirKeyLight;
        private static Light dirTopLight;
        private static Light dirFillLight;
        
        private static Light makerBackLight;

        private static ConfigEntry<bool> faceLightsControl { get; set; }
        private static ConfigEntry<float> faceLightsIntensity { get; set; }
        private static ConfigEntry<Color> faceLightsColor { get; set; }
        
        private static ConfigEntry<bool> backLightControl { get; set; }
        private static ConfigEntry<float> backLightIntensity { get; set; }
        private static ConfigEntry<Color> backLightColor { get; set; }

        private static ConfigEntry<bool> camLightControl { get; set; }
        private static ConfigEntry<float> camLightIntensity { get; set; }
        private static ConfigEntry<Color> camLightColor { get; set; }
        
        
        private static ConfigEntry<bool> dirBackLightControl { get; set; }
        private static ConfigEntry<float> dirBackLightIntensity { get; set; }
        private static ConfigEntry<Color> dirBackLightColor { get; set; }
        
        private static ConfigEntry<bool> dirKeyLightControl { get; set; }
        private static ConfigEntry<float> dirKeyLightIntensity { get; set; }
        private static ConfigEntry<Color> dirKeyLightColor { get; set; }

        private static ConfigEntry<bool> dirTopLightControl { get; set; }
        private static ConfigEntry<float> dirTopLightIntensity { get; set; }
        private static ConfigEntry<Color> dirTopLightColor { get; set; }

        private static ConfigEntry<bool> dirFillLightControl { get; set; }
        private static ConfigEntry<float> dirFillLightIntensity { get; set; }
        private static ConfigEntry<Color> dirFillLightColor { get; set; }

        
        private static ConfigEntry<bool> makerBackLightControl { get; set; }
        private static ConfigEntry<float> makerBackLightIntensity { get; set; }
        private static ConfigEntry<Color> makerBackLightColor { get; set; }

        private void Update()
        {
            if (backLightControl.Value && backLight != null)
            {
                backLight.intensity = backLightIntensity.Value;
                backLight.color = backLightColor.Value;
            }

            if (camLightControl.Value && camLight != null)
            {
                camLight.intensity = camLightIntensity.Value;
                camLight.color = camLightColor.Value;
            }
            
            if (makerBackLightControl.Value && makerBackLight != null)
            {
                makerBackLight.intensity = makerBackLightIntensity.Value;
                makerBackLight.color = makerBackLightColor.Value;
            }

            if (dirBackLightControl.Value && dirBackLight != null)
            {
                dirBackLight.intensity = dirBackLightIntensity.Value;
                dirBackLight.color = dirBackLightColor.Value;
            }
            
            if (dirFillLightControl.Value && dirFillLight != null)
            {
                dirFillLight.intensity = dirFillLightIntensity.Value;
                dirFillLight.color = dirFillLightColor.Value;
            }
            
            if (dirKeyLightControl.Value && dirKeyLight != null)
            {
                dirKeyLight.intensity = dirKeyLightIntensity.Value;
                dirKeyLight.color = dirKeyLightColor.Value;
            }
            
            if (dirTopLightControl.Value && dirTopLight != null)
            {
                dirTopLight.intensity = dirTopLightIntensity.Value;
                dirTopLight.color = dirTopLightColor.Value;
            }

            if (!faceLightsControl.Value || faceLights.Count == 0) return;
            
            foreach (var faceLight in faceLights.Where(faceLight => faceLight != null))
            {
                faceLight.intensity = faceLightsIntensity.Value;
                faceLight.color = faceLightsColor.Value;
            }
        }
        
        private void Awake()
        {
            faceLights = new List<Light>();
            
            faceLightsControl = Config.AddSetting(new ConfigDefinition("Facelights", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            faceLightsIntensity = Config.AddSetting(new ConfigDefinition("Facelights", INTENSITY), 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            faceLightsColor = Config.AddSetting(new ConfigDefinition("Facelights", COLOR), new Color(1, 1, 1), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            backLightControl = Config.AddSetting(new ConfigDefinition("Backlight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            backLightIntensity = Config.AddSetting(new ConfigDefinition("Backlight", INTENSITY), 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            backLightColor = Config.AddSetting(new ConfigDefinition("Backlight", COLOR), new Color(1, 1, 1), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            camLightControl = Config.AddSetting(new ConfigDefinition("Camlight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            camLightIntensity = Config.AddSetting(new ConfigDefinition("Camlight", INTENSITY), 0.55f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            camLightColor = Config.AddSetting(new ConfigDefinition("Camlight", COLOR), new Color(1, 1, 1), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            
            dirBackLightControl = Config.AddSetting(new ConfigDefinition("Dir Backlight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            dirBackLightIntensity = Config.AddSetting(new ConfigDefinition("Dir Backlight", INTENSITY), 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            dirBackLightColor = Config.AddSetting(new ConfigDefinition("Dir Backlight", COLOR), new Color(0.748f, 0.727f, 0.688f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            dirFillLightControl = Config.AddSetting(new ConfigDefinition("Dir FillLight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            dirFillLightIntensity = Config.AddSetting(new ConfigDefinition("Dir FillLight", INTENSITY), 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            dirFillLightColor = Config.AddSetting(new ConfigDefinition("Dir FillLight", COLOR), new Color(0.407f, 0.469f, 0.510f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            dirKeyLightControl = Config.AddSetting(new ConfigDefinition("Dir KeyLight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            dirKeyLightIntensity = Config.AddSetting(new ConfigDefinition("Dir KeyLight", INTENSITY), 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            dirKeyLightColor = Config.AddSetting(new ConfigDefinition("Dir KeyLight", COLOR), new Color(0.396f, 0.542f, 0.991f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            dirTopLightControl = Config.AddSetting(new ConfigDefinition("Dir TopLight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            dirTopLightIntensity = Config.AddSetting(new ConfigDefinition("Dir TopLight", INTENSITY), 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            dirTopLightColor = Config.AddSetting(new ConfigDefinition("Dir TopLight", COLOR), new Color(0.700f, 0.674f, 0.642f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            
            makerBackLightControl = Config.AddSetting(new ConfigDefinition("Maker Backlight", ENABLE), false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
            makerBackLightIntensity = Config.AddSetting(new ConfigDefinition("Maker Backlight", INTENSITY), 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            makerBackLightColor = Config.AddSetting(new ConfigDefinition("Maker Backlight", COLOR), new Color(0.470f, 0.599f, 0.650f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));

            HarmonyWrapper.PatchAll(typeof(AI_LightSettings));
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.ActorController), "InitializeFaceLight")]
        public static void ActorController_InitializeFaceLight_Postfix(AIProject.ActorController __instance)
        {
            if (__instance.FaceLight == null) return;
            faceLights.Add(__instance.FaceLight);
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.ActorCameraControl), "Start")]
        public static void ActorCameraControl_Start_Postfix(AIProject.ActorCameraControl __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            GameObject N_Light = traverse.Field("_charaLightNormal").GetValue<GameObject>();
            GameObject N_Light_Custom = traverse.Field("_charaLightCustom").GetValue<GameObject>();

            if (N_Light != null)
            {
                Transform BackLight = N_Light.transform.Find("Back Light");
                Transform CamLight = N_Light.transform.Find("Cam Light");

                if (BackLight != null)
                {
                    Light comp = BackLight.gameObject.GetComponent<Light>();

                    if (comp != null)
                        backLight = comp;
                }

                if (CamLight != null)
                {
                    Light comp = CamLight.gameObject.GetComponent<Light>();

                    if (comp != null)
                        camLight = comp;
                }
            }
            
            if (N_Light_Custom != null)
            {
                Transform Back = N_Light_Custom.transform.Find("Directional Light Back");
                Transform Top = N_Light_Custom.transform.Find("Directional Light Top");
                Transform Fill = N_Light_Custom.transform.Find("Directional Light Fill");
                Transform Key = N_Light_Custom.transform.Find("Directional Light Key");

                if (Back != null)
                {
                    Light comp = Back.gameObject.GetComponent<Light>();

                    if (comp != null)
                        dirBackLight = comp;
                }

                if (Top != null)
                {
                    Light comp = Top.gameObject.GetComponent<Light>();

                    if (comp != null)
                        dirTopLight = comp;
                }
                
                if (Fill != null)
                {
                    Light comp = Fill.gameObject.GetComponent<Light>();

                    if (comp != null)
                        dirFillLight = comp;
                }

                if (Key != null)
                {
                    Light comp = Key.gameObject.GetComponent<Light>();

                    if (comp != null)
                        dirKeyLight = comp;
                }
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(CharaCustom.CharaCustom), "Start")]
        public static void CharaCustom_Start_Postfix(CharaCustom.CharaCustom __instance)
        {
            Transform MakerBackLight = __instance.transform.Find("CustomControl/CharaCamera/Main Camera/Lights Custom/Directional Light Back");

            if (MakerBackLight == null) return;
            
            Light comp = MakerBackLight.gameObject.GetComponent<Light>();

            if (comp != null)
                makerBackLight = comp;
        }
    }
}
