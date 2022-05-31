using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MetaScripts
{
    public class ExitGameScript : MonoBehaviour
    {
        [SerializeField] private GameObject exitConfirmationCanvas;
        [SerializeField] private PostProcessProfile postProcessing;
        [SerializeField] private float baseFocusDistance;
        [SerializeField] private GameObject mainCanvas;

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) return;
            postProcessing.TryGetSettings(out DepthOfField depthOfField);
            depthOfField.focusDistance.value = 0.1f;
            mainCanvas.SetActive(false);
            exitConfirmationCanvas.SetActive(true);
        }

        public void QuitGame() => Application.Quit();

        public void CancelQuit()
        {
            postProcessing.TryGetSettings(out DepthOfField depthOfField);
            depthOfField.focusDistance.value = baseFocusDistance;
            exitConfirmationCanvas.SetActive(false);
            mainCanvas.SetActive(true);
        }
    }
}