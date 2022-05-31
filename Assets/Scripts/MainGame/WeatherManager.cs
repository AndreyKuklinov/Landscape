using System;
using UnityEngine;
using Random = UnityEngine.Random;
using RenderSettings = UnityEngine.RenderSettings;

namespace MainGame
{
    public class WeatherManager : MonoBehaviour
    {
        [SerializeField] private GameObject directionalLightGameObject;
        [SerializeField] private Material noRainSkybox;
        [SerializeField] private Material yesRainSkybox;
        [SerializeField] private float lightDecadenceSpeed;

        private bool isItRaining;
        private int gameStartTime;
        private int rainStartTime;
        private Light directionalLight;

        private void Start()
        {
            gameStartTime = DateTime.Now.Minute;
            directionalLight = directionalLightGameObject.GetComponent<Light>();
        }

        private void Update()
        {
            if (DateTime.Now.Minute - Random.Range(2, 6) >= gameStartTime && !isItRaining)
            {
                rainStartTime = DateTime.Now.Minute;
                isItRaining = true;
                gameStartTime = rainStartTime;
                RenderSettings.skybox = yesRainSkybox;
            }

            if (DateTime.Now.Minute - Random.Range(1, 2) >= rainStartTime && isItRaining)
            {
                isItRaining = false;
                gameStartTime = DateTime.Now.Minute;
                RenderSettings.skybox = noRainSkybox;
            }

            if (isItRaining && directionalLight.intensity > 0)
                directionalLight.intensity -= lightDecadenceSpeed * Time.deltaTime;
            if (!isItRaining && directionalLight.intensity < 1)
                directionalLight.intensity += lightDecadenceSpeed * Time.deltaTime;
        }
    }
}