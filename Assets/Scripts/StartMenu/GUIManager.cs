using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text ScoreText;

    private void Start()
    {
        ScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
    }
}
