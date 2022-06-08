using System;
using MainGame;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class CameraSettingsManager : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private Slider sensitivitySlider;


        private void Start()
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("SensitivityMouse");
        }

        public void ChangeMouseSensitivity()
        {
            camera.gameObject.GetComponent<SimpleCameraController>().mouseRotationMaxSpeed = sensitivitySlider.value;
            PlayerPrefs.SetFloat("SensitivityMouse", sensitivitySlider.value);
        }
    }
}