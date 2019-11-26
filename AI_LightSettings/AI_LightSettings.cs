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
        public const string VERSION = "2.0.0";

        private static List<Light> faceLights;
        private static Light makerBackLight;
        private static Light backLight;
        private static Light camLight;
        
        private static ConfigEntry<bool> faceLightsControl { get; set; }
        private static ConfigEntry<float> faceLightsIntensity { get; set; }
        private static ConfigEntry<Color> faceLightsColor { get; set; }
        
        private static ConfigEntry<bool> backLightControl { get; set; }
        private static ConfigEntry<float> backLightIntensity { get; set; }
        private static ConfigEntry<Color> backLightColor { get; set; }

        private static ConfigEntry<bool> camLightControl { get; set; }
        private static ConfigEntry<float> camLightIntensity { get; set; }
        private static ConfigEntry<Color> camLightColor { get; set; }
        
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
            
            faceLightsControl = Config.AddSetting(new ConfigDefinition("Facelights", "Enable Settings"), false, new ConfigDescription("Enable custom control settings.", null, new ConfigurationManagerAttributes { Order = 3 }));
            faceLightsIntensity = Config.AddSetting(new ConfigDefinition("Facelights", "Intensity"), 0.5f, new ConfigDescription("Intensity of the Facelights.", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            faceLightsColor = Config.AddSetting(new ConfigDefinition("Facelights", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Facelights.", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            backLightControl = Config.AddSetting(new ConfigDefinition("Backlight", "Enable Settings"), false, new ConfigDescription("Enable custom control settings.", null, new ConfigurationManagerAttributes { Order = 3 }));
            backLightIntensity = Config.AddSetting(new ConfigDefinition("Backlight", "Intensity"), 0.5f, new ConfigDescription("Intensity of the Backlight.", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            backLightColor = Config.AddSetting(new ConfigDefinition("Backlight", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Backlight.", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            camLightControl = Config.AddSetting(new ConfigDefinition("Camlight", "Enable Settings"), false, new ConfigDescription("Enable custom control settings.", null, new ConfigurationManagerAttributes { Order = 3 }));
            camLightIntensity = Config.AddSetting(new ConfigDefinition("Camlight", "Intensity"), 0.55f, new ConfigDescription("Intensity of the Camlight.", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            camLightColor = Config.AddSetting(new ConfigDefinition("Camlight", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Camlight.", null, new ConfigurationManagerAttributes { Order = 1 }));
            
            makerBackLightControl = Config.AddSetting(new ConfigDefinition("Maker Backlight", "Enable Settings"), false, new ConfigDescription("Enable custom control settings.", null, new ConfigurationManagerAttributes { Order = 3 }));
            makerBackLightIntensity = Config.AddSetting(new ConfigDefinition("Maker Backlight", "Intensity"), 1f, new ConfigDescription("Intensity of the Maker Backlight.", new AcceptableValueRange<float>(0f, 1f), null, new ConfigurationManagerAttributes { Order = 2 }));
            makerBackLightColor = Config.AddSetting(new ConfigDefinition("Maker Backlight", "Color"), new Color(0.470f, 0.599f, 0.650f), new ConfigDescription("Color of the Maker Backlight.", null, new ConfigurationManagerAttributes { Order = 1 }));

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

            if (N_Light == null) return;

            Transform BackLight = N_Light.transform.GetChild(0);
            Transform CamLight = N_Light.transform.GetChild(1);

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
