using System;
using TreeEditor;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Water.Scripts;

namespace StartMenu
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Material day;
        [SerializeField] private Material night;
        [SerializeField] private GameObject currentOcean;
        [SerializeField] private GameObject oceanDay;
        [SerializeField] private GameObject oceanNight;
        [SerializeField] private Text[] texts;
    

        private void Awake()
        {
            var currentDate = DateTime.Now;
            if (currentDate.Hour > 6 && currentDate.Hour < 20)
            {
                RenderSettings.skybox = day;
                foreach (var text in texts)
                {
                    text.color = Color.black;
                }

                currentOcean = oceanDay;
            }
            else
            {
                RenderSettings.skybox = night;
                foreach (var text in texts)
                {
                    text.color = Color.white;
                }

                currentOcean = oceanNight;
            }

        }

        private void Start()
        {
            scoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
        
        }
    
    
    }
}
