using UnityEngine;

namespace ScenesFolders.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private void SetDifficulty(string newDifficulty)
        {
            PlayerPrefs.SetString("Difficulty", newDifficulty);
        }
    
        private string GetDifficulty()
        {
            return PlayerPrefs.GetString("Difficulty");
        }
    
        private void SetVolume(int newVolume)
        {
            PlayerPrefs.SetInt("Volume", newVolume);
        }
    
        private int GetVolume()
        {
            return PlayerPrefs.GetInt("Volume");
        }
    }
}
