using System;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider boardSizeSlider;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject postProcessing;
        [SerializeField] private Toggle postProcessingToggle;


        public void CloseSettings() =>
            canvas.SetActive(false);

        public void OpenSettings() =>
            canvas.SetActive(true);

        private void Start()
        {
            postProcessingToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("postProcessing"));
            musicSlider.value = MusicVolume;
            boardSizeSlider.value = PlayerPrefs.GetInt("boardWidth");
        }

        private float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public void ChangeMusicVolume() =>
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);


        public void ChangeBoardSize() =>
            PlayerPrefs.SetInt("boardWidth", (int) boardSizeSlider.value);

        public void ChangePostProcessing()
        {
            var isActive = postProcessingToggle.isOn;
            PlayerPrefs.SetInt("postProcessing", isActive ? 1 : 0);
            postProcessing.SetActive(isActive);
        }
    }
}