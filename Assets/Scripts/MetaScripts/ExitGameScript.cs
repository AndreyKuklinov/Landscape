using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MetaScripts
{
    public class ExitGameScript : MonoBehaviour, IPopUp
    {
        [SerializeField] private GameObject exitConfirmationCanvas;
        [SerializeField] private PostProcessProfile postProcessing;
        [SerializeField] private float baseFocusDistance;
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private GameObject[] otherPopups;
        [SerializeField] private Canvas myPopUpCanvas;
        public bool isIgnoringEsc;

        private void Start()
        {
            postProcessing.TryGetSettings(out DepthOfField depthOfField);
            depthOfField.focusDistance.value = baseFocusDistance;
        }

        private void Update()
        {
            if (!Input.GetKeyUp(KeyCode.Escape)) return;
            if (!(otherPopups is null) && otherPopups.Any(popup => popup.activeSelf)) return;
            if (isIgnoringEsc && !myPopUpCanvas.isActiveAndEnabled) return;
            if (exitConfirmationCanvas.activeSelf)
                CancelQuit();
            else
                OpenQuitUI();
            //я понимаю что надо шину ивентов делать и подписываться на ивангая, но уже поздно, оставлю это как тудушку
        }

        public void QuitGame() => Application.Quit();

        public void CancelQuit()
        {
            postProcessing.TryGetSettings(out DepthOfField depthOfField);
            depthOfField.focusDistance.value = baseFocusDistance;
            exitConfirmationCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            if (!(otherPopups is null) && otherPopups.Any(popup => popup.activeSelf))
                foreach (var popup in otherPopups)
                    popup.transform.parent.GetComponent<IPopUp>().DontIgnoreEsc();
        }

        public void OpenQuitUI()
        {
            postProcessing.TryGetSettings(out DepthOfField depthOfField);
            depthOfField.focusDistance.value = 0.1f;
            mainCanvas.SetActive(false);
            exitConfirmationCanvas.SetActive(true);
            if (!(otherPopups is null) && otherPopups.Any(popup => popup.activeSelf))
                foreach (var popup in otherPopups)
                    popup.transform.parent.GetComponent<IPopUp>().IgnoreEsc();
        }

        public bool IsIgnoringEsc() => isIgnoringEsc;
        public void IgnoreEsc() => isIgnoringEsc = true;

        public void DontIgnoreEsc() => isIgnoringEsc = false;
    }
}