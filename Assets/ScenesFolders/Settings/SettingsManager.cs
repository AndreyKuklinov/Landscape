using UnityEngine;

namespace ScenesFolders.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public string Difficulty
        {
            set => PlayerPrefs.SetString("Difficulty", value);
            get => PlayerPrefs.GetString("Difficulty");
        }

        public int MusicVolume
        {
            set => PlayerPrefs.SetInt("MusicVolume", value);
            get => PlayerPrefs.GetInt("MusicVolume");
        }
        public int InterfaceVolume
        {
            set => PlayerPrefs.SetInt("InterfaceVolume", value);
            get => PlayerPrefs.GetInt("InterfaceVolume");
        }

    }
}