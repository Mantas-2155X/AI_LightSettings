using BepInEx;
using BepInEx.Configuration;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace AI_LightSettings {
    [BepInPlugin(nameof(AI_LightSettings), nameof(AI_LightSettings), "1.01")]
    public class AI_LightSettings : BaseUnityPlugin {
        
        private Light backLight;
        public static ConfigEntry<float> backLightIntensity { get; private set; }

        void SceneChanged(Scene oldScene, Scene newScene) {
            GameObject backLightObj = GameObject.Find("Directional Light Back");
            
            if (backLightObj != null) {
                Light lightComp = backLightObj.GetComponent<Light>();

                if (lightComp != null) {
                    backLight = lightComp;
                }
            }
        }

        void Awake() {
            backLightIntensity = Config.AddSetting(new ConfigDefinition("Backlight", "Intensity"), 1f, new ConfigDescription("Intensity of the Backlight.", new AcceptableValueRange<float>(0f, 1f)));
        }
        
        void Update() {
            if (backLight != null) {
                backLight.intensity = backLightIntensity.Value;
            }
        }

        void OnEnable() {
            SceneManager.activeSceneChanged += SceneChanged;
        }

        void OnDisable() {
            SceneManager.activeSceneChanged -= SceneChanged;
        }
        
    }
}
