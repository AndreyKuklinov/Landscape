using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider boardSizeSlider;
        [SerializeField] private GameObject canvas;


        public void CloseSettings() =>
            canvas.SetActive(false);

        public void OpenSettings() =>
            canvas.SetActive(true);

        private void Start()
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        public string Difficulty
        {
            set => PlayerPrefs.SetString("Difficulty", value);
            get => PlayerPrefs.GetString("Difficulty");
        }

        public float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public void ChangeMusicVolume() =>
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        
        public void ChangeBoardSize() =>
            PlayerPrefs.SetFloat("boardWidth", boardSizeSlider.value);
        
    }
}