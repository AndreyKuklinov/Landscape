using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider interfaceSlider;


        private void Start()
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            interfaceSlider.value = PlayerPrefs.GetFloat("InterfaceVolume");
        }

        public string Difficulty
        {
            set => PlayerPrefs.SetString("Difficulty", value);
            get => PlayerPrefs.GetString("Difficulty");
        }

        public float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public int InterfaceVolume => PlayerPrefs.GetInt("InterfaceVolume");

        public void ChangeMusicVolume()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }

        public void ChangeInterfaceVolume()
        {
            PlayerPrefs.SetFloat("InterfaceVolume", interfaceSlider.value);
        }
    }
}