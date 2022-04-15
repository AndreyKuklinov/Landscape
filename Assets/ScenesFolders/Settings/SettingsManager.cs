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

        public int Volume
        {
            set => PlayerPrefs.SetInt("Volume", value);
            get => PlayerPrefs.GetInt("Volume");
        }
    }
}