using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Slider musicVolumeScroll;
        [SerializeField] private Slider interfaceVolumeScroll;
        public string Difficulty
        {
            set => PlayerPrefs.SetString("Difficulty", value);
            get => PlayerPrefs.GetString("Difficulty");
        }

        public float MusicVolume => PlayerPrefs.GetFloat("MusicVolume");

        public int InterfaceVolume
        {
            set => PlayerPrefs.SetInt("InterfaceVolume", value);
            get => PlayerPrefs.GetInt("InterfaceVolume");
        }

        public void ChangeMusicVolume()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeScroll.value); 
        }
        
        public void ChangeInterfaceVolume()
        {
            PlayerPrefs.SetFloat("InterfaceVolume", interfaceVolumeScroll.value);
        }

    }
}