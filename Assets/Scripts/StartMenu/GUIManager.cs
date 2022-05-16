using System;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private int dayStartTime;
        [SerializeField] private int dayEndTime;
    

        private void Awake()
        {
            var currentDate = DateTime.Now;
            if (currentDate.Hour > dayStartTime && currentDate.Hour < dayEndTime)
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
