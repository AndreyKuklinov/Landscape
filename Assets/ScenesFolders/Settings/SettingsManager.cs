using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private void SetDifficulty(string newDifficulty)
    {
        PlayerPrefs.SetString("Difficulty", newDifficulty)
    }
    
    private string GetDifficulty()
    {
        PlayerPrefs.GetString("Difficulty")
    }
    
    private void SetVolume(int newVolume)
    {
        PlayerPrefs.SetInt("Volume", newVolume)
    }
    
    private int GetVolume()
    {
        PlayerPrefs.GetInt("Volume")
    }

    
}
