using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StartMenu
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Image slider;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private float fillSpeed;
        [SerializeField] private ParticleSystem particleSystem;

        private float Progress;

        private void Start() =>
            scoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();


        private void Update()
        {
            if (slider.fillAmount < Progress)
                slider.fillAmount += fillSpeed * Time.deltaTime;
        }

        public void StartGameAsync()
        {
            loadingScreen.SetActive(true);
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            var sceneLoadingOperation = SceneManager.LoadSceneAsync(1);
            while (!sceneLoadingOperation.isDone)
            {
                Progress = sceneLoadingOperation.progress;
                yield return null;
            }
        }
    }
}