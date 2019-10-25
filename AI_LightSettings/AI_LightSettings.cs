using HarmonyLib;

using BepInEx;
using BepInEx.Harmony;
using BepInEx.Configuration;

using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace AI_LightSettings {
    [BepInPlugin(nameof(AI_LightSettings), nameof(AI_LightSettings), "pre2.0.0")]
    public class AI_LightSettings : BaseUnityPlugin
    {
        private static List<Light> faceLights;
        private static Light makerBackLight;
        private static Light backLight;
        private static Light camLight;
        
        private static ConfigEntry<float> faceLightsIntensity { get; set; }
        private static ConfigEntry<Color> faceLightsColor { get; set; }
        
        private static ConfigEntry<float> backLightIntensity { get; set; }
        private static ConfigEntry<Color> backLightColor { get; set; }
        
        private static ConfigEntry<float> makerBackLightIntensity { get; set; }
        private static ConfigEntry<Color> makerBackLightColor { get; set; }
        
        private static ConfigEntry<float> camLightIntensity { get; set; }
        private static ConfigEntry<Color> camLightColor { get; set; }

        private void Update()
        {
            if (backLight != null)
            {
                backLight.intensity = backLightIntensity.Value;
                backLight.color = backLightColor.Value;
            }
            
            if (makerBackLight != null)
            {
                makerBackLight.intensity = makerBackLightIntensity.Value;
                makerBackLight.color = makerBackLightColor.Value;
            }

            if (camLight != null)
            {
                camLight.intensity = camLightIntensity.Value;
                camLight.color = camLightColor.Value;
            }

            if (faceLights.Count == 0) return;
            
            foreach (var faceLight in faceLights.Where(faceLight => faceLight != null))
            {
                faceLight.intensity = faceLightsIntensity.Value;
                faceLight.color = faceLightsColor.Value;
            }
        }
        
        private void Awake()
        {
            faceLights = new List<Light>();
            
            faceLightsIntensity = Config.AddSetting(new ConfigDefinition("Facelights", "Intensity"), 0.5f, new ConfigDescription("Intensity of the Facelights.", new AcceptableValueRange<float>(0f, 1f)));
            faceLightsColor = Config.AddSetting(new ConfigDefinition("Facelights", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Facelights."));
            
            backLightIntensity = Config.AddSetting(new ConfigDefinition("Backlight", "Intensity"), 0.5f, new ConfigDescription("Intensity of the Backlight.", new AcceptableValueRange<float>(0f, 1f)));
            backLightColor = Config.AddSetting(new ConfigDefinition("Backlight", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Backlight."));
            
            makerBackLightIntensity = Config.AddSetting(new ConfigDefinition("Maker Backlight", "Intensity"), 1f, new ConfigDescription("Intensity of the Maker Backlight.", new AcceptableValueRange<float>(0f, 1f)));
            makerBackLightColor = Config.AddSetting(new ConfigDefinition("Maker Backlight", "Color"), new Color(0.470f, 0.599f, 0.650f), new ConfigDescription("Color of the Maker Backlight."));

            camLightIntensity = Config.AddSetting(new ConfigDefinition("Camlight", "Intensity"), 0.55f, new ConfigDescription("Intensity of the Camlight.", new AcceptableValueRange<float>(0f, 1f)));
            camLightColor = Config.AddSetting(new ConfigDefinition("Camlight", "Color"), new Color(1, 1, 1), new ConfigDescription("Color of the Camlight."));
            
            HarmonyWrapper.PatchAll(typeof(AI_LightSettings));
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.ActorController), "InitializeFaceLight")]
        [UsedImplicitly]
        public static void ActorController_InitializeFaceLight_Postfix(AIProject.ActorController __instance)
        {
            if (__instance.FaceLight == null) return;
            faceLights.Add(__instance.FaceLight);
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.ActorCameraControl), "Start")]
        [UsedImplicitly]
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
        [UsedImplicitly]
        public static void CharaCustom_Start_Postfix(CharaCustom.CharaCustom __instance)
        {
            if (__instance == null) return;

            Transform MakerBackLight = __instance.transform.Find("CustomControl/CharaCamera/Main Camera/Lights Custom/Directional Light Back");

            if (MakerBackLight == null) return;
            
            Light comp = MakerBackLight.gameObject.GetComponent<Light>();

            if (comp != null)
                makerBackLight = comp;
        }

    }
}
