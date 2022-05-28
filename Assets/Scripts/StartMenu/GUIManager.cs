using System.Collections;
using MetaScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StartMenu
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject creditCanvas;

        private void Start() =>
            scoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();

        public void StartGame()
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadScene(1);
        }

        public void ShowCredits() => creditCanvas.SetActive(true);
        public void HideCredits() => creditCanvas.SetActive(false);
    }
}