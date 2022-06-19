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

        private void Awake()
        {
            postProcessingToggle.isOn = !Convert.ToBoolean(PlayerPrefs.GetInt("NOTpostProcessing"));
            musicSlider.value = MusicVolume;
            if (fxToggle != null)
                fxToggle.isOn = !Convert.ToBoolean(PlayerPrefs.GetInt("NOTFX"));
        }

        private float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public void ChangeMusicVolume() =>
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        public void ChangePostProcessing()
        {
            var isActive = postProcessingToggle.isOn;
            PlayerPrefs.SetInt("NOTpostProcessing", isActive ? 0 : 1);
            postProcessing.SetActive(isActive);
        }

        public void SwitchFX()
        {
            var isFXOn = fxToggle.isOn;
            PlayerPrefs.SetInt("NOTFX", isFXOn ? 0 : 1);
            foreach (var tile in tilePrefabs)
                for (var i = 0; i < tile.transform.childCount; i++)
                    if (tile.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>())
                        tile.transform.GetChild(i).gameObject.SetActive(isFXOn);
        }
    }
}