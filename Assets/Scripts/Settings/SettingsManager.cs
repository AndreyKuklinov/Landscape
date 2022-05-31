using System;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject postProcessing;
        [SerializeField] private Toggle postProcessingToggle;
        [SerializeField] private Toggle fxToggle;
        [SerializeField] private GameObject[] tilePrefabs;


        public void CloseSettings() =>
            canvas.SetActive(false);

        public void OpenSettings() =>
            canvas.SetActive(true);

        private void Start()
        {
            postProcessingToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("postProcessing"));
            musicSlider.value = MusicVolume;
            fxToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FX"));
        }

        private float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public void ChangeMusicVolume() =>
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        public void ChangePostProcessing()
        {
            var isActive = postProcessingToggle.isOn;
            PlayerPrefs.SetInt("postProcessing", isActive ? 1 : 0);
            postProcessing.SetActive(isActive);
        }

        public void SwitchFX()
        {
            var isFXOn = fxToggle.isOn;
            PlayerPrefs.SetInt("FX", isFXOn ? 1 : 0);
            foreach (var tile in tilePrefabs)
                for (var i = 0; i < tile.transform.childCount; i++)
                    if (tile.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>())
                        tile.transform.GetChild(i).gameObject.SetActive(isFXOn);
        }
    }
}